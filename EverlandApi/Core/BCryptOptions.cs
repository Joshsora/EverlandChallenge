using BCrypt;

namespace EverlandApi.Core
{
    public class BCryptOptions
    {
        public int SaltWorkFactor { get; set; }
        public SaltRevision SaltRevision { get; set; }

        public BCryptOptions()
        {
            SaltWorkFactor = 10;
            SaltRevision = SaltRevision.Revision2B;
        }
    }
}
