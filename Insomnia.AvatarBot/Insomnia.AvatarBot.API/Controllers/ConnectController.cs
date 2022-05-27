using AutoMapper;
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

namespace Insomnia.AvatarBot.API.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class ConnectController : BaseController
    {
        private readonly ILogger<ConnectController> _logger;
        private readonly IMapper _mapper;
        private readonly BotConfig _config;

        public ConnectController(ILogger<ConnectController> logger, IMapper mapper, BotConfig config)
        {
            _logger = logger;
            _mapper = mapper;
            _config = config;
        }

        [HttpGet("alive")]
        public async Task<IActionResult> Alive([)
        {
            Ok("Ты наливаешь воду в")
        }

        [HttpPost("callback")]
        public async Task<IActionResult> Callback([FromBody] Updates model)
        {
            if (model.SecretKey == _config.SecretKey)
            {
                return await CheckCommand(model);
            }

            return Ok();
        }

        private async Task<IActionResult> CheckCommand(Updates model) =>
            model.Type switch
            {
                MessageType.Confirmation => Confirmation(),
                MessageType.NewMessage => Ok(),
                _ => Default()
            };

        private IActionResult Confirmation()
        {
            return Ok(_config.Confirmation);
        }

        private IActionResult Default()
        {
            return Ok(BotResponse.Default);
        }
    }
}
