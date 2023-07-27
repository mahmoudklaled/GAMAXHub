﻿using Business.Posts.Services;
using DataBase.Core.Models.Reacts;
using GAMAX.Services.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GAMAX.Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RaectController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IReactServices _reactServices;
        public RaectController(IHttpContextAccessor httpContextAccessor, IReactServices reactServices)
        {
            _httpContextAccessor = httpContextAccessor;
            _reactServices = reactServices;
        }
        [HttpPost("DeletePostReact")]
        public async  Task<IActionResult> DeletePostReact(Guid reactId)
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            var result = await _reactServices.DeletePostReactAsync(reactId, userInfo.Email);
            if(result)
                return Ok(result);
            return BadRequest(new
            {
                Message = "Fail"
            }); 
        }
        [HttpPost("DeleteQuestionPostReact")]
        public async Task<IActionResult> DeleteQuestionPostReact(Guid reactId)
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            var result = await _reactServices.DeleteQuestionPostReactAsync(reactId, userInfo.Email);
            if (result)
                return Ok(result);
            return BadRequest(new
            {
                Message = "Fail"
            });
        }
        [HttpPost("DeleteCommentPostReact")]
        public async Task<IActionResult> DeleteCommentPostReact(Guid reactId)
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            var result = await _reactServices.DeleteCommentPostReactAsync(reactId, userInfo.Email);
            if (result)
                return Ok(result);
            return BadRequest(new
            {
                Message = "Fail"
            });
        }
        [HttpPost("DeleteCommentQuestionReact")]
        public async Task<IActionResult> DeleteCommentQuestionReact(Guid reactId)
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            var result = await _reactServices.DeleteCommentQuestionReactAsync(reactId, userInfo.Email);
            if (result)
                return Ok(result);
            return BadRequest(new
            {
                Message = "Fail"
            });
        }
        [HttpPost(" AddReactOnPost")]
        public async Task<IActionResult> AddReactOnPost(ReactRequest reactRequest)
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            var result = await _reactServices.AddReactOnPostAsync(reactRequest, userInfo.Email);
            if (result)
                return Ok(result);
            return BadRequest(new
            {
                Message = "Fail"
            });
        }
        [HttpPost(" AddReactOnQuestionPost")]
        public async Task<IActionResult> AddReactOnQuestionPost(ReactRequest reactRequest)
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            var result = await _reactServices.AddReactOnQuestionPostAsync(reactRequest, userInfo.Email);
            if (result)
                return Ok(result);
            return BadRequest(new
            {
                Message = "Fail"
            });
        }
        [HttpPost(" AddReactOnPostComment")]
        public async Task<IActionResult> AddReactOnPostComment(ReactRequest reactRequest)
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            var result = await _reactServices.AddReactOnPostCommentAsync(reactRequest, userInfo.Email);
            if (result)
                return Ok(result);
            return BadRequest(new
            {
                Message = "Fail"
            });
        }
        [HttpPost(" AddReactOnQuestionPostComment")]
        public async Task<IActionResult> AddReactOnQuestionPostComment(ReactRequest reactRequest)
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            var result = await _reactServices.AddReactOnQuestionPostCommentAsync(reactRequest, userInfo.Email);
            if (result)
                return Ok(result);
            return BadRequest(new
            {
                Message = "Fail"
            });
        }
        [HttpPost(" UpdatePostReact")]
        public async Task<IActionResult> UpdatePostReact(ReactRequest reactRequest)
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            var result = await _reactServices.UpdatePostReact(reactRequest, userInfo.Email);
            if (result)
                return Ok(result);
            return BadRequest(new
            {
                Message = "Fail"
            });
        }
        [HttpPost(" UpdateQuestionReact")]
        public async Task<IActionResult> UpdateQuestionReact(ReactRequest reactRequest)
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            var result = await _reactServices.UpdateQuestionReact(reactRequest, userInfo.Email);
            if (result)
                return Ok(result);
            return BadRequest(new
            {
                Message = "Fail"
            });
        }
        [HttpPost(" UpdatePostCommentReact")]
        public async Task<IActionResult> UpdatePostCommentReact(ReactRequest reactRequest)
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            var result = await _reactServices.UpdatePostCommentReact(reactRequest, userInfo.Email);
            if (result)
                return Ok(result);
            return BadRequest(new
            {
                Message = "Fail"
            });
        }
        [HttpPost(" UpdateQuestionCommentReact")]
        public async Task<IActionResult> UpdateQuestionCommentReact(ReactRequest reactRequest)
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            var result = await _reactServices.UpdateQuestionCommentReact(reactRequest, userInfo.Email);
            if (result)
                return Ok(result);
            return BadRequest(new
            {
                Message = "Fail"
            });
        }
        
    }
}
