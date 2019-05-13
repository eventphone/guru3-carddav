using System;
using System.Linq;

namespace eventphone.guru3.carddav.DAL
{
    public static class Extensions
    {
        public static IQueryable<Event> Active(this IQueryable<Event> source)
        {
            var now = DateTime.Now.Date;
            return source.Where(x => x.RegistrationStart != null || x.IsPermanentAndPublic)
                .Where(x => x.RegistrationStart <= now || x.IsPermanentAndPublic)
                .Where(x => x.End != null || x.IsPermanentAndPublic)
                .Where(x => x.End > now || x.IsPermanentAndPublic);
        }

        public static IQueryable<Extension> Active(this IQueryable<Extension> source)
        {
            var now = DateTime.Now.Date;
            return source.Where(x => x.Event.RegistrationStart != null || x.Event.IsPermanentAndPublic)
                .Where(x => x.Event.RegistrationStart <= now || x.Event.IsPermanentAndPublic)
                .Where(x => x.Event.End != null || x.Event.IsPermanentAndPublic)
                .Where(x => x.Event.End > now || x.Event.IsPermanentAndPublic)
                .Where(x=>x.InPhonebook);
        }
    }
}