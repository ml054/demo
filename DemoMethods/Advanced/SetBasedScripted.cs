﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DemoMethods.Indexes;
using Raven.Abstractions.Data;
using DemoMethods.Entities;

namespace DemoMethods.Advanced
{
    public partial class AdvancedController : ApiController
    {
        [HttpGet]
        public object SetBasedScripted(string original = "USA", string newVal = "United States of America")
        {
            var updateByIndex =
                DocumentStoreHolder.Store.DatabaseCommands.UpdateByIndex(new CompaniesAndCountry().IndexName,
                    new IndexQuery {Query = "Address_Country:" + original},
                    new ScriptedPatchRequest
                    {
                        Script = "this.Address.Country = newVal;",
                        Values = new Dictionary<string, object> {{"newVal", newVal}}
                    });

            updateByIndex.WaitForCompletion();


            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                var results = session
                    .Query<CompaniesAndCountry.Result, CompaniesAndCountry>()
                    .Customize(x => x.WaitForNonStaleResultsAsOfNow())
                    .Where(x => x.Address_Country == newVal)
                    .OfType<Company>()
                    .ToList();

                return (results);
            }
        }
    }
}