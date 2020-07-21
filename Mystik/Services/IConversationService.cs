﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mystik.Entities;

namespace Mystik.Services
{
    public interface IConversationService : IDisposable
    {
        Task<bool> IsTheConversationManager(Guid conversationId, Guid userId);
        Task AddMembers(Guid id, IEnumerable<Guid> users);
        Task DeleteMembers(Guid conversationId, List<Guid> usersIds);
        Task<Conversation> Create(string name, byte[] passwordHashData, Guid userId);
        Task<Conversation> Retrieve(Guid id);
        Task<IReadOnlyList<string>> ChangeName(Guid id, string newName);
        Task<IReadOnlyList<string>> Delete(Guid id);
        Task<IEnumerable<Conversation>> GetAll();
    }
}
