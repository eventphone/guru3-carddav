using System;
using System.Linq;

namespace eventphone.guru3.carddav.DAL
{
    public static class Extensions
    {
        public static IQueryable<Event> Active(this IQueryable<Event> source)
        {
            var now = DateTime.Now.Date;
            return source.Where(x => x.RegistrationStart != null)
                .Where(x => x.RegistrationStart <= now)
                .Where(x => x.End != null)
                .Where(x => x.End > now);
        }

        public static IQueryable<Extension> Active(this IQueryable<Extension> source)
        {
            var now = DateTime.Now.Date;
            return source.Where(x => x.Event.RegistrationStart != null)
                .Where(x => x.Event.RegistrationStart <= now)
                .Where(x => x.Event.End != null)
                .Where(x => x.Event.End > now)
                .Where(x=>x.InPhonebook);
        }
    }
}