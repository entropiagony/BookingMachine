using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class WorkPlace
    {
        public int Id { get; set; }
        [JsonIgnore]
        public Floor Floor { get; set; }
        public int FloorId { get; set; }

    }
}
