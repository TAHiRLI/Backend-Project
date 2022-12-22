
using Quarter.DAL;

namespace Quarter.Services
{
    public class LayoutService
    {
        private readonly QuarterDbContext _context;

        public LayoutService(QuarterDbContext context)
        {
            this._context = context;
        }
        public Dictionary<string, string> GetSettings()
        {
            return _context.Settings.ToDictionary(x => x.Key, x => x.Value);

        }
    }
}
