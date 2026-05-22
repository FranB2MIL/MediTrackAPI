using Infrastructure.Persistance;

namespace Infrastructure.Repositories
{
    public class MedicRepository
    {
        private readonly MediTrackDbContext _context;
        public MedicRepository(MediTrackDbContext context)
        {
            _context = context;
        }

        
    }
}