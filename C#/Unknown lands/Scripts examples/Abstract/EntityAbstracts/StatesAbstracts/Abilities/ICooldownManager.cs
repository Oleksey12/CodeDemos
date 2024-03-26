using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Project.Scripts {
    public interface ICooldownManager {
        public void ReduceAllCooldowns(float value);

        public void ReduceCooldown(string cooldownName, float value);

        public void ResetAllCooldowns();

        public void ResetCooldown(string cooldownName);

        public bool IsCooldownReady(string cooldownName);
    }
}
