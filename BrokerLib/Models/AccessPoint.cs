using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using UtilsLib.Utils;

namespace BrokerLib.Models
{
    public class AccessPoint : DBModelBase
    {
        [Key]
        [Column(TypeName = "nvarchar(450)")]
        public string id { get; set; }
        [Column(TypeName = "nvarchar(200)")]
        public string Name { get; set; }
        [Column(TypeName = "nvarchar(200)")]
        public string Account { get; set; }
        [Column(TypeName = "nvarchar(200)")]
        public string PublicKey { get; set; }
        [Column(TypeName = "nvarchar(500)")]
        public string PrivateKey { get; set; }
        [Column(TypeName = "nvarchar(500)")]
        public string BearerToken { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public string UserId { get; set; }
        [Column(TypeName = "bigint")]
        public int BrokerId { get; set; }

        public AccessPoint()
        : base(BrokerDBContext.providers)
        {

        }

        public override void Store()
        {
            try
            {
                id = Guid.NewGuid().ToString();
                base.Store();
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
        }

        public static AccessPoint Construct(string accessPointId)
        {
            try
            {
                AccessPoint ap = BrokerDBContext.Execute(context => {
                    return context.AccessPoints.Find(accessPointId);
                });

                if (ap == null)
                {
                    BrokerLib.DebugMessage(String.Format("AccessPoint::Constructor() : accessPointId {0} is not valid", accessPointId));
                    return null;
                }

                return ap;
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return null;
        }
    }
}
