﻿using Business.Services;
using DataBase.Core;
using DataBase.Core.Enums;
using DataBase.Core.Models.Notifications;
using DataBase.Core.Models.Posts;
using DomainModels;
using DomainModels.DTO;
using DomainModels.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilites;

namespace Business.Implementation
{
    public class NotificationServices : INotificationServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly SignalRActions _signalRActions;
        public NotificationServices(IUnitOfWork unitOfWork, SignalRActions signalRActions)
        {
            _unitOfWork = unitOfWork;
            _signalRActions = signalRActions;
        }
        public void AddNotification(NotificationDTO notification)
        {
            var _notification = new Notifications
            {
                Id = Guid.NewGuid(),
                ActionedUserId = notification.ActionedUserId,
                ItemId = notification.ItemId,
                NotificatinType = notification.NotificatinType,
                NotifiedUserId = notification.NotifiedUserId,
            };
            Task.Run(async () =>
            {
                await _unitOfWork.Notification.AddAsync(_notification);
                await _unitOfWork.Complete();
            });
            return;
        }
        public async Task<IEnumerable<NotificationDTO>> GetAllUserNotifications(Guid userId)
        {
            string[] includes = { "ActionedUser" };
            var notifications = await _unitOfWork.Notification.FindAllAsync(n => n.NotifiedUserId == userId, includes);
            var notificationDTO = OMapper.Mapper.Map<List<DomainModels.DTO.NotificationDTO>>(notifications);
            return notificationDTO;
        }
        public void NotifyOnAddingComment(CommentDTO commentDTO, Guid postId, PostsTypes postsType)
        {
            Task.Run(() => {
                Guid userPostOwnerId;
                var notificationDTO = new NotificationDTO()
                {
                    ActionedUserId = commentDTO.UserId,
                    ActionUserFirstName = commentDTO.UserFirstName,
                    ActionUserLastName = commentDTO.UserLastName,
                    ItemId = postId,
                };
                if (postsType == PostsTypes.Post)
                {
                    notificationDTO.NotificatinType = NotificatinTypes.AddComment;
                    userPostOwnerId = _unitOfWork.Post.Find(p => p.Id == postId).UserAccountsId;
                }
                else
                {
                    notificationDTO.NotificatinType = NotificatinTypes.AddQuestion;
                    userPostOwnerId = _unitOfWork.QuestionPost.Find(p => p.Id == postId).UserAccountsId;
                }
                AddNotification(notificationDTO);
                var notificationModel = new DomainModels.DTO.NotificationModel
                {
                    ActionedUserId = notificationDTO.ActionedUserId,
                    ActionUserFirstName = notificationDTO.ActionUserFirstName,
                    ActionUserLastName = notificationDTO.ActionUserLastName,
                    NotifiedUserId = notificationDTO.NotifiedUserId,
                    TimeCreated = TimeHelper.ConvertTimeCreateToString(DateTime.UtcNow),
                    PostId = notificationDTO.ItemId,
                    PostsType = postsType,
                   NotificatinType = notificationDTO.NotificatinType,
                };
                SendCommentNotification(notificationModel);

            });

            return;
        }
        public void NotifyOnAddingPost(PostDTO postDTO)
        {
            var notificationDTO = new NotificationDTO()
            {
                ActionedUserId = postDTO.UserId,
                ActionUserFirstName = postDTO.UserFirstName,
                ActionUserLastName = postDTO.UserLastName,
                ItemId = postDTO.Id,
                NotificatinType = NotificatinTypes.AddPost
            };
            var notificationModel = new DomainModels.DTO.NotificationModel
            {
                ActionedUserId = postDTO.UserId,
                ActionUserFirstName = postDTO.UserFirstName,
                ActionUserLastName = postDTO.UserLastName,
                NotifiedUserId = postDTO.UserId,
                TimeCreated= postDTO.TimeCreated,
                PostId = postDTO.Id,
                PostsType=PostsTypes.Post,
                NotificatinType = notificationDTO.NotificatinType,
            };
            Task.Run(async () => {  await SendPostNotification(notificationModel); });
            return;
        }
        public void NotifyOnAddingQuestion(QuestionPostDTO postDTO)
        {
            var notificationDTO = new NotificationDTO()
            {
                ActionedUserId = postDTO.UserId,
                ActionUserFirstName = postDTO.UserFirstName,
                ActionUserLastName = postDTO.UserLastName,
                ItemId = postDTO.Id,
                NotificatinType = NotificatinTypes.AddQuestion
            };
            var notificationModel = new DomainModels.DTO.NotificationModel
            {
                ActionedUserId = postDTO.UserId,
                ActionUserFirstName = postDTO.UserFirstName,
                ActionUserLastName = postDTO.UserLastName,
                NotifiedUserId = postDTO.UserId,
                TimeCreated = postDTO.TimeCreated,
                PostId = postDTO.Id,
                PostsType = PostsTypes.Question,
                NotificatinType = notificationDTO.NotificatinType,
            };
            Task.Run(async () => {await SendPostNotification(notificationModel); });
        }
        public void RemoveAllUserNotification(Guid Id)
        {
            Task.Run(async () =>
            {
                var notifications = await _unitOfWork.Notification.FindAllAsync(n => n.NotifiedUserId == Id);
                _unitOfWork.Notification.DeleteRange(notifications);
                await _unitOfWork.Complete();
            });
            return;
        }
        public void NotifyOnApproveFriendRequest(Guid ApprovedUserId, Guid userId)
        {
            Task.Run(async () => {
                var accountProfile = await _unitOfWork.UserAccounts.FindAsync(p => p.Id == userId);
                var userAccount = new DomainModels.DTO.UserAccount
                {
                    Id = accountProfile.Id,
                    Email = accountProfile.Email,
                    UserName = accountProfile.UserName,
                    FirstName = accountProfile.FirstName,
                    LastName = accountProfile.LastName,
                    City = accountProfile.City,
                    Country = accountProfile.Country,
                    Bio = accountProfile.Bio,
                    Birthdate = accountProfile.Birthdate,
                    gender = accountProfile.gender.ToString(),
                    Type = accountProfile.Type.ToString(),
                };

                ApproveFriendRequestNotification(ApprovedUserId, userAccount);
            });
        }

        public void NotifyOnSendFriendRequest(Guid RecivedUserId, Guid userId)
        {
            Task.Run(async () => {
                var accountProfile = await _unitOfWork.UserAccounts.FindAsync(p => p.Id == userId);
                var userAccount = new DomainModels.DTO.UserAccount
                {
                    Id = accountProfile.Id,
                    Email = accountProfile.Email,
                    UserName = accountProfile.UserName,
                    FirstName = accountProfile.FirstName,
                    LastName = accountProfile.LastName,
                    City = accountProfile.City,
                    Country = accountProfile.Country,
                    Bio = accountProfile.Bio,
                    Birthdate = accountProfile.Birthdate,
                    gender = accountProfile.gender.ToString(),
                    Type = accountProfile.Type.ToString(),
                };

                SendFriendRequestNotification(RecivedUserId, userAccount);
            });
        }
        public void NotifyOnAddingReactPost(ReactsDTO reactDTO, AddReactRequest reactRequest)
        {
            Task.Run( () => {
                var notificationDTO = new NotificationDTO()
                {
                    ActionedUserId = reactDTO.UserId,
                    ActionUserFirstName = reactDTO.UserFirstName,
                    ActionUserLastName = reactDTO.UserLastName,
                    ItemId = reactRequest.ObjectId,
                    NotificatinType = NotificatinTypes.AddReactOnPost
                };
                var notificationModel = new DomainModels.DTO.NotificationModel
                {
                    ActionedUserId = reactDTO.UserId,
                    ActionUserFirstName = reactDTO.UserFirstName,
                    ActionUserLastName = reactDTO.UserLastName,
                    NotifiedUserId = _unitOfWork.Post.Find(p => p.Id == reactRequest.ObjectId).UserAccountsId,
                    TimeCreated = TimeHelper.ConvertTimeCreateToString(DateTime.UtcNow),
                    PostId = reactRequest.ObjectId,
                    PostsType = PostsTypes.Post,
                    NotificatinType = notificationDTO.NotificatinType,
                };
                AddNotification(notificationDTO); 
                SendReactNotificationOnPost(notificationModel);
            });
        }
        public void NotifyOnAddingReactQuestionPost(ReactsDTO reactDTO, AddReactRequest reactRequest)
        {
            Task.Run(() =>
            {
                var notificationDTO = new NotificationDTO()
                {
                    ActionedUserId = reactDTO.UserId,
                    ActionUserFirstName = reactDTO.UserFirstName,
                    ActionUserLastName = reactDTO.UserLastName,
                    ItemId = reactRequest.ObjectId,
                    NotificatinType = NotificatinTypes.AddReactOnQuestion
                };
                var notificationModel = new DomainModels.DTO.NotificationModel
                {
                    ActionedUserId = reactDTO.UserId,
                    ActionUserFirstName = reactDTO.UserFirstName,
                    ActionUserLastName = reactDTO.UserLastName,
                    NotifiedUserId = _unitOfWork.QuestionPost.Find(p => p.Id == reactRequest.ObjectId).UserAccountsId,
                    TimeCreated = TimeHelper.ConvertTimeCreateToString(DateTime.UtcNow),
                    PostId = reactRequest.ObjectId,
                    PostsType = PostsTypes.Question,
                    NotificatinType = notificationDTO.NotificatinType,
                };
                AddNotification(notificationDTO);
                SendReactNotificationOnPost(notificationModel);
            });
        }
        //public void NotifyOnAddingReactOnComment(ReactsDTO reactDTO, AddReactRequest reactRequest)
        //{
        //    Task.Run(async () => {
        //        var notificationDTO = new NotificationDTO()
        //        {
        //            ActionedUserId = reactDTO.UserId,
        //            ActionUserFirstName = reactDTO.UserFirstName,
        //            ActionUserLastName = reactDTO.UserLastName,
        //            ItemId = reactRequest.ObjectId,
        //            NotificatinType = NotificatinTypes.AddReactOnComment
        //        };
        //        var notificationModel = new DomainModels.DTO.NotificationModel
        //        {
        //            ActionedUserId = reactDTO.UserId,
        //            ActionUserFirstName = reactDTO.UserFirstName,
        //            ActionUserLastName = reactDTO.UserLastName,
        //            NotifiedUserId = _unitOfWork.PostComment.Find(p => p.Id == reactRequest.ObjectId).UserAccountsId,
        //            TimeCreated = TimeHelper.ConvertTimeCreateToString(DateTime.UtcNow),
        //            PostId = _unitOfWork.PostComment.Find(p => p.Id == reactRequest.ObjectId).PostId,
        //            PostsType = PostsTypes.Post,
        //            NotificatinType = notificationDTO.NotificatinType,
        //        };
        //        AddNotification(notificationDTO);
        //        await SendPostNotification(notificationModel);
        //    });
        //}
        //public void NotifyOnAddingReactOnAnswer(ReactsDTO reactDTO, AddReactRequest reactRequest)
        //{
        //    Task.Run(() => {
        //        var notificationDTO = new NotificationDTO()
        //        {
        //            ActionedUserId = reactDTO.UserId,
        //            ActionUserFirstName = reactDTO.UserFirstName,
        //            ActionUserLastName = reactDTO.UserLastName,
        //            ItemId = reactRequest.ObjectId,
        //            NotificatinType = NotificatinTypes.AddReactOnPost
        //        };
        //        var notificationModel = new DomainModels.DTO.NotificationModel
        //        {
        //            ActionedUserId = reactDTO.UserId,
        //            ActionUserFirstName = reactDTO.UserFirstName,
        //            ActionUserLastName = reactDTO.UserLastName,
        //            NotifiedUserId = _unitOfWork.Post.Find(p => p.Id == reactRequest.ObjectId).UserAccountsId,
        //            TimeCreated = TimeHelper.ConvertTimeCreateToString(DateTime.UtcNow),
        //            PostId = reactRequest.ObjectId,
        //            PostsType = PostsTypes.Post,
        //            NotificatinType = notificationDTO.NotificatinType,
        //        };
        //        AddNotification(notificationDTO);
        //        SendReactNotificationOnPost(notificationModel);
        //    });
        //}
        private async Task SendPostNotification(DomainModels.DTO.NotificationModel notification)
        {
            await _signalRActions.OnAddingPostAction?.Invoke(notification);
        }
        private void SendCommentNotification(DomainModels.DTO.NotificationModel notification)
        {
            _signalRActions.OnAddingCommentAction?.Invoke(notification);
        }
        private void SendReactNotificationOnPost(DomainModels.DTO.NotificationModel notification)
        {
            _signalRActions.OnAddingReactOnPostAction?.Invoke(notification);
        }
        private void SendFriendRequestNotification(Guid RecivedUserId, UserAccount userAccount)
        {
            _signalRActions.OnSendingFriendRequestAction?.Invoke(RecivedUserId, userAccount);
        }
        private void ApproveFriendRequestNotification(Guid ApprovedUserId, UserAccount userAccount)
        {
            _signalRActions.OnApprovedFriendRequestAction?.Invoke(ApprovedUserId, userAccount);
        }

        
    }
}