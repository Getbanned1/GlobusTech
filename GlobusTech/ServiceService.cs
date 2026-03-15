using GlobusTech;

namespace GlobusTech.Services
{
    public class ServiceService
    {
        public List<Service> GetServices(
            IEnumerable<Service> services,
            string? searchText,
            int sortIndex)
        {
            var query = services.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var search = searchText.ToLower();
                query = query.Where(s => s.Name.ToLower().Contains(search));
            }

            switch (sortIndex)
            {
                case 1: // Новые
                    query = query.OrderBy(s => s.StartDate);
                    break;
                case 2: // Старые
                    query = query.OrderByDescending(s => s.StartDate);
                    break;
            }

            return query.ToList();
        }
    }
}
