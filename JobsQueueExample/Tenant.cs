using System.Collections.Generic;

namespace JobsQueueExample
{
    public class Tenant
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Name => "Tenant " + Id;
        public List<Content> Todos { get; set; } = new List<Content>();
    }

    public static class TenantGenerator
    {
        // generate tree of tenants
        public static IEnumerable<Tenant> GenerateTenants(int count = 5)
        {
            var tenants = new List<Tenant>();
            for (var id = 1; id <= count; id++)
            {
                tenants.Add(new Tenant
                {
                    Id = id,
                    ParentId = id - 1
                });
            }
            return tenants;
        }
    }

    public static class TenantRepository
    {
        private readonly static List<Tenant> _tenants = TenantGenerator.GenerateTenants().ToList();
        public static IEnumerable<Tenant> GetAll()
        {
            return _tenants;
        }

        // get tenant by id
        public static Tenant? GetById(int id)
        {
            return _tenants.FirstOrDefault(t => t.Id == id);
        }

        public static IEnumerable<Tenant> GetChildren(int parentId)
        {
            return _tenants.Where(t => t.ParentId == parentId);
        }

        // tenants count property
        public static int Count => _tenants.Count;
    }
}
