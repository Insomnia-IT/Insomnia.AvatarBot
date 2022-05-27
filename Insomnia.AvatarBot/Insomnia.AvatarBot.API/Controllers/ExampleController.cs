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

namespace Insomnia.AvatarBot.API.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class ExampleController : BaseController
    {
        private readonly ILogger<ExampleController> _logger;
        private readonly IMapper _mapper;

        public ExampleController(ILogger<ExampleController> logger, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            return Ok();
        }
    }
}
