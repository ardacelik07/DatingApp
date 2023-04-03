using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Entities;
using API.Helpers;

namespace API.interfaces
{
    public interface IMessageRepository
    {
        void AddMessage(Messages message);
         void DeleteMessage(Messages message);
         Task<Messages> GetMessage(int id);
         Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);

         Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName,string recipientUserName);

         Task<bool> SaveAllAsync();
    }
}