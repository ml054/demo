﻿using System;
using DemoCommon.Models;
using DemoCommon.Utils.Database;

namespace DemoServer.Utils.Database
{
    public class DatabaseLinks
    {
        private readonly DatabaseSettings _databaseSettings;
        private readonly DatabaseName _databaseName;

        public DatabaseLinks(Settings settings, DatabaseName databaseName)
        {
            _databaseName = databaseName;
            _databaseSettings = settings.Database;
        }

        private string FirstDatabaseUrl => _databaseSettings.Urls[0];

        private string StudioUrl => $"{FirstDatabaseUrl}/studio/index.html";

        private string GetDocumentsUrl(string databaseName) => $"{StudioUrl}#databases/documents?&database={databaseName}";

        private string GetIndexesUrl(string databaseName) => $"{StudioUrl}#databases/indexes?&database={databaseName}";

        public string ToUserDocuments(Guid userId)
        {
            var databaseName = _databaseName.For(userId);
            return GetDocumentsUrl(databaseName);
        }

        public string ToUserIndexes(Guid userId)
        {
            var databaseName = _databaseName.For(userId);
            return GetIndexesUrl(databaseName);
        }

        public string ToMediaDocuments(Guid userId)
        {
            var databaseName = _databaseName.MediaFor(userId);
            return GetDocumentsUrl(databaseName);
        }
    }
}
