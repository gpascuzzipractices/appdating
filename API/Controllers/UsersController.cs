using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using API.Extensions;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
   
    [Authorize]//meglio in alto livello
    public class UsersController:BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        public UsersController(IUserRepository userRepository, IMapper mapper,IPhotoService photoService)
        {
            _mapper=mapper;
            _userRepository=userRepository;
            _photoService=photoService;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await _userRepository.GetMembersAsync();                     
            return Ok(users);
        }


        
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await _userRepository.GetMemberAsync(username);                    
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
             //var username = User.GetUsername();
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            if (user == null) return NotFound();
            _mapper.Map(memberUpdateDto, user);
            if (await _userRepository.SaveAllAsync()) return NoContent(); 
            return BadRequest("Non sono riuscito ad aggiornare l'utente");
        }


        
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto (IFormFile file)
        {
           var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
           if (user == null) return NotFound();
           var result = await _photoService.AddPhotoAsync(file);
           if (result.Error != null) return BadRequest(result.Error.Message);
           var photo = new Photo
           {
               Url=result.SecureUrl.AbsoluteUri,
               PublicId=result.PublicId
           };
           if (user.Photos.Count == 0) photo.IsMain=true;
           user.Photos.Add(photo);
           if (await _userRepository.SaveAllAsync())
           {
               return CreatedAtAction(nameof(GetUser),
                new {username=user.UserName}, _mapper.Map<PhotoDto>(photo));
           }
           return BadRequest("Problemi per il caricamento della photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            if (user == null) return NotFound();
            var photo = user.Photos.FirstOrDefault(x=>x.Id==photoId);
            if(photo==null) return NotFound();
            if (photo.IsMain) return BadRequest("è già la tua foto principale");
            var currentMain = user.Photos.FirstOrDefault(x=>x.IsMain);
            if (currentMain!=null) currentMain.IsMain=false;
            photo.IsMain=true;
            if (await _userRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Errore per il set di foto");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            if (user == null) return NotFound();
            var photo = user.Photos.FirstOrDefault(x=>x.Id==photoId);
            if (photo==null) return NotFound();
            if (photo.IsMain) return BadRequest("Non puoi eliminare la tua foto principale");
            if (photo.PublicId!=null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error!=null) return BadRequest(result.Error.Message);
            }
            user.Photos.Remove(photo);
            if (await _userRepository.SaveAllAsync()) return Ok();
            return BadRequest("Problemi per la rimozione della foto");

        }


    }
}