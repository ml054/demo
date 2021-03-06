﻿using System.Threading.Tasks;
using DemoCommon.Models;
using DemoServer.Utils.Cache;
using DemoServer.Utils.Database;
using DemoServer.Utils.UserId;
using Microsoft.AspNetCore.Mvc;
#region Usings
using System.Linq;
using System.Collections.Generic;
using Raven.Client.Documents.Session;
#endregion

namespace DemoServer.Controllers.Demos.Queries.FullCollectionQuery
{
    public class FullCollectionQueryController : DemoCodeController
    {
        public FullCollectionQueryController(UserIdContainer userId, UserStoreCache userStoreCache, MediaStoreCache mediaStoreCache,
            DatabaseSetup databaseSetup) : base(userId, userStoreCache, mediaStoreCache, databaseSetup)
        {
        }

        private async Task SetRunPrerequisites()
        {
            List<Company> documentsToStore = new List<Company>
            {
                new Company {Id = "companies/1", Name = "Name1", Phone = "Phone1"},
                new Company {Id = "companies/2", Name = "Name2", Phone = "Phone2"},
                new Company {Id = "companies/3", Name = "Name3", Phone = "Phone3"},
                new Company {Id = "companies/4", Name = "Name4", Phone = "Phone4"},
                new Company {Id = "companies/5", Name = "Name5", Phone = "Phone5"}
            };

            await DatabaseSetup.EnsureUserCollectionExists(UserId, documentsToStore);
        }
        
        [HttpPost]
        public async Task<IActionResult> Run()
        {
            await SetRunPrerequisites();
            
            #region Demo
            List<Company> collectionResults;

            using (IDocumentSession session = DocumentStoreHolder.Store.OpenSession())
            {
                #region Step_1
                IQueryable<Company> fullCollectionQuery = session.Query<Company>();
                #endregion
                
                #region Step_2
                collectionResults = fullCollectionQuery.ToList();
                #endregion
            }
            
            #endregion 
            
            return Ok(collectionResults);
        }
    }
}
