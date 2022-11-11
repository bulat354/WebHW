using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;
using System.Net;

namespace MyServer
{
    public class SessionManager
    {
        public static SessionManager Instance { get; } = new SessionManager();

        private MemoryCache _sessions;
        private CacheItemPolicy SlidingPolicy
        {
            get
            {
                var policy = new CacheItemPolicy();
                policy.SlidingExpiration = TimeSpan.FromMinutes(2);
                return policy;
            }
        }
        private CacheItemPolicy AbsolutePolicy
        {
            get
            {
                var policy = new CacheItemPolicy();
                policy.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(10);
                return policy;
            }
        }

        private SessionManager()
        {
            _sessions = MemoryCache.Default;
        }

        public Session CreateSession(int accountId, string email)
        {
            var guid = Guid.NewGuid().ToString();
            var session = new Session(guid, accountId, email);

            while (_sessions.Contains(guid))
            {
                var tmp = (Session)_sessions.Get(guid);
                if (tmp.AccountId == accountId)
                {
                    _sessions.Set(guid, session, SlidingPolicy);
                    return session;
                }
                guid = Guid.NewGuid().ToString();
            }

            _sessions.Add(guid, session, SlidingPolicy);
            return session;
        }

        public bool CheckSession(string guid)
        {
            return _sessions.Contains(guid);
        }

        public Session GetSession(string guid)
        {
            if (CheckSession(guid))
                return (Session)_sessions.Get(guid);

            throw new ArgumentException("Session with the same guid doesn't exist");
        }
    }

    public struct Session
    {
        public string Id { get; }
        public int AccountId { get; }
        public string Email { get; }
        public DateTime Created { get; }

        public Session(string id, int accountId, string email)
        {
            Id = id;
            AccountId = accountId;
            Email = email;
            Created = DateTime.Now;
        }
    }
}
