using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.Dtos;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class MessagesController : BaseApiController
    {
        public IUserRepository _UserRepository { get; }
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository,IMapper mapper)
        {
            _mapper = mapper;
            _messageRepository = messageRepository;
            _UserRepository = userRepository;
        }

        [HttpPost]
       public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto){
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if(username == createMessageDto.RecipientUsername.ToLower())
            return BadRequest("You cannot send messages to yourself");
               
             var sender = await _UserRepository.GetUserByUsernameAsync(username);
             var Recipient = await _UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);
             if( Recipient == null) return NotFound();

             var message = new Messages
             {
                   Sender = sender,
                   Recipient = Recipient,
                   SenderUsername = sender.UserName,
                   RecipientUsername = Recipient.UserName,
                   Content = createMessageDto.Content
             };
             _messageRepository.AddMessage(message);

             if(await _messageRepository.SaveAllAsync())
             return Ok(_mapper.Map<MessageDto>(message));

             return BadRequest("failed to send Message");
       }
       [HttpGet]

       public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
       {
                   messageParams.Username =    User.FindFirst(ClaimTypes.Name)?.Value;

                   var messages = await _messageRepository.GetMessagesForUser(messageParams);

                   Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage,messages.PageSize,messages.TotalCount,messages.TotalPages));

                   return messages;

       }
       [HttpGet("thread/{username}")]

       public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username){
        var currentUserName = User.FindFirst(ClaimTypes.Name)?.Value;

        return Ok(await _messageRepository.GetMessageThread(currentUserName,username));
       }
       [HttpDelete("{id}")]
       public async Task<ActionResult> DeleteMessage(int id){

         var username = User.FindFirst(ClaimTypes.Name)?.Value;

         var message = await _messageRepository.GetMessage(id);

         if(message.SenderUsername != username && message.RecipientUsername != username) return Unauthorized();
         if(message.SenderUsername == username) message.SenderDeleted = true;
         if(message.RecipientUsername == username) message.RecipientDeleted = true;
         if(message.SenderDeleted && message.RecipientDeleted){
            _messageRepository.DeleteMessage(message);
         }

         if(await _messageRepository.SaveAllAsync()) return Ok();
         return BadRequest("problem deleting the message");

       }
    }
}