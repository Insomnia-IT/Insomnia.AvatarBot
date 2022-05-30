﻿using AutoMapper;
using AutoMapper.Collection;
using AutoMapper.EntityFrameworkCore;
using Insomnia.AvatarBot.API.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Insomnia.AvatarBot.Data.ViewModels.Input;
using Insomnia.AvatarBot.BI.Options;
using Insomnia.AvatarBot.Data.Messages;
using VkNet.Model;
using VkNet.Model.RequestParams;
using VkNet.Utils;
using System.IO;
using Insomnia.AvatarBot.BI.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VkNet.UWP.Model.Attachments;
using Insomnia.AvatarBot.Data.Dto;
using VkNet.Abstractions;
using System.Text;
using System.Net;
using VkNet;
using VkNet.Enums.SafetyEnums;

namespace Insomnia.AvatarBot.API.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class VkController : ControllerBase
    {
        private readonly ILogger<VkController> _logger;
        private readonly IMapper _mapper;
        private readonly BotConfig _config;
        private readonly ICommands _commands;
        private readonly IVkApi _vkApi;

        public VkController(ILogger<VkController> logger, IMapper mapper, ICommands commands)
        {
            _logger = logger;
            _mapper = mapper;
            _config = new BotConfig();
            _commands = commands;
            _vkApi = new VkApi();
            _vkApi.Authorize(new ApiAuthParams { AccessToken = "c5a27bf0e24ae3aeed24b82cfd1c78fc883d2d670faa1951586431ddaf2e3144edd8ebd24f0681daea4fb" });
        }

        private static List<BotHistory> Messages = new List<BotHistory>();

        [HttpGet("alive")]
        public async Task<IActionResult> Alive()
        {
            return Ok("Ты наливаешь воду в бутылку она становится бутылкой...");
        }

        [HttpPost("command")]
        public async Task<IActionResult> Command([FromBody] Updates model)
        {
            if (model.SecretKey == _config.SecretKey)
            {
                return await CheckCommand(model);
            }

            return Ok();
        }

        [HttpPost("generate-image")]
        public async Task<IActionResult> GenerateImageToStream(IFormFile file, [FromForm] int number)
        {
            var result = await _commands.GenerateImageToStream(file, number);

            return GetFile(result);
        }

        [HttpPost("generate-image-for-url")]
        public async Task<IActionResult> GenerateImageToStream([FromForm] string url, [FromForm] int number)
        {
            var result = await _commands.GenerateImage(url, number);

            return GetFile(result);
        }

        private IActionResult GetFile(Stream file)
        {
            return base.File((file as MemoryStream).ToArray(), "application/octet-stream", "example.jpg");
        }

        private async Task<IActionResult> CheckCommand(Updates model) =>
            model.Type switch
            {
                MessageType.Confirmation => Confirmation(),
                MessageType.NewMessage => await NewMessage(model.Object, model.GroupId),
                _ => Default()
            };

        private async Task<IActionResult> NewMessage(JObject message, long groupId)
        {
            var msg = Message.FromJson(new VkResponse(message));

            if (!msg.FromId.HasValue)
                return Default();

            switch (msg.Text.ToLower())
            {
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                case "10":
                   return await GenerateFrame(msg, groupId);
                case "хочу крутую аватарку":
                case "хочу крутую аватарку!":
                case "показать шаблоны":
                    return SendFrames(msg.PeerId, msg.FromId);
                default:

                    if (msg.Attachments.Count() != 1)
                    {
                        return Default();
                    };

                    return await GenerateFrame(msg, groupId);
            }
        }

        private async Task<IActionResult> GenerateFrame(Message msg, long groupId)
        {
            var history = Messages.SingleOrDefault(m => m.UserId == msg.FromId.ToString());

            if (history is null)
            {
                history = new BotHistory() { UserId = msg.FromId.ToString() };

                Messages.Add(history);
            }

            if (!String.IsNullOrEmpty(msg.Text))
                history.Number = int.Parse(msg.Text);
            else
                history.Url = (msg.Attachments.First().Instance as VkNet.Model.Attachments.Photo).Sizes.OrderByDescending(x => x.Height).First().Url.AbsoluteUri;

            if (String.IsNullOrEmpty(history.Url))
            {
                return SendMessage(@$"Почти готово! Скоро будет сгенерирована крутейшая аватарка по {msg.Text}-му шаблону :)
Отправьте пожалуйста фотографию которую желаете украсить!

Учтите: фотография будет обрезана по центру!
А так же не советуем отправлять маленькие изображения, иначе они будут некрасиво растянуты.", msg.FromId.Value);
            }
            else
            {
                if ((DateTime.Now - history.TimeLastCreate).TotalSeconds < 3)
                    return Ok();

                else
                {
                    var photo = await _commands.GenerateImage(history.Url, history.Number);

                    if (photo is null)
                        return SendMessage("О нет! Что-то пошло не так. Наши специалисты уже разбираются. Попробуйте загрузить фото позднее.", msg.PeerId.Value);

                    Messages.Remove(history);

                    return await SendMessage(photo, msg.FromId.Value, groupId);
                }
            }
        }

        private IActionResult SendFrames(long? pearId, long? fromId)
        {
            /*  var albumid = 123456789;
              var photos = _vkApi.Photo.Get(new PhotoGetParams
              {
                  AlbumId = PhotoAlbumType.Id(albumid),
                  OwnerId = _vkApi.UserId.Value
              });
            */
            var pId = pearId.HasValue ? pearId.Value : 0;
            var fId = fromId.HasValue ? fromId.Value : 0; 

            if(fId != 0)
                SendMessage($"fromId {fId}, pearId {pId}", fId);
            if(pId != 0)
                SendMessage($"pearId {pId}, fromId {fId}", pId);
            return Ok();
        }

        private IActionResult Confirmation()
        {
            return Ok(_config.Confirmation);
        }

        private IActionResult Default()
        {
            return Ok();
        }

        private IActionResult SendMessage(string message, long peerId)
        {
            _vkApi.Messages.Send(new MessagesSendParams
            {
                RandomId = new DateTime().Millisecond,
                PeerId = peerId,
                Message = message
            });

            return Ok();
        }

        private async Task<IActionResult> SendMessage(Stream photo, long peerId, long groupId)
        {
            return SendMessage(await UploadImage(photo, groupId), peerId);
        }

        private async Task<IEnumerable<VkNet.Model.Attachments.MediaAttachment>> UploadImage(Stream photo, long groupId)
        {
            var uploadServer = await _vkApi.Photo.GetMessagesUploadServerAsync(groupId);
            var wc = new WebClient();

            var result = Encoding.ASCII.GetString(wc.UploadData(uploadServer.UploadUrl, (photo as MemoryStream).ToArray()));

            return _vkApi.Photo.SaveMessagesPhoto(result);
        }

        private IActionResult SendMessage(string message, IEnumerable<VkNet.Model.Attachments.MediaAttachment> attachments, long peerId)
        {
            _vkApi.Messages.Send(new MessagesSendParams
            {
                RandomId = new DateTime().Millisecond,
                Message = message,
                PeerId = peerId,
                Attachments = attachments
            });

            return Ok();
        }

        private IActionResult SendMessage(IEnumerable<VkNet.Model.Attachments.MediaAttachment> attachments, long peerId)
        {
            _vkApi.Messages.Send(new MessagesSendParams
            {
                RandomId = new DateTime().Millisecond,
                PeerId = peerId,
                Attachments = attachments
            });

            return Ok();
        }
    }
}
