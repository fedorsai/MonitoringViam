namespace PressMachineServices.Database
{
    using System.Data.Entity;
    using System.Linq;
    using PressMachineServices.Press;

    public class PressMachineDbContex : DbContext
    {
        public PressMachineDbContex() : base("PressMachineDbContex")
        {
            this.Configuration.AutoDetectChangesEnabled = false;
            this.Configuration.ValidateOnSaveEnabled = false;
        }

        public DbSet<PressOperationData> PressOperationDatas { get; set; }

        public DbSet<Press> Presses { get; set; }
    }
}
