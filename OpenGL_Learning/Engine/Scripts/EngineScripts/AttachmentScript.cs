using OpenGL_Learning.Engine.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Mathematics;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_Learning.Engine.Scripts.EngineScripts
{
    public class AttachmentScript: Script
    {
        GameWorldObject ownerGW = null;
        public GameWorldObject attachementParent = null;

        // Parameters
        public Vector3 attachmentPositionMask = Vector3.One;
        public Vector3 attachmentPositionOffset = Vector3.Zero;

        // ----

        public AttachmentScript() { }

        protected override void OnScriptAttached()
        {
            base.OnScriptAttached();

            if (owner is GameWorldObject) ownerGW = (GameWorldObject)owner;
        }

        protected override void OnScriptUpdated(float deltaTime)
        {
            base.OnScriptUpdated(deltaTime);

            if (ownerGW != null && attachementParent != null) 
            {
                Vector3 targetLocation = attachementParent.location + attachmentPositionOffset;

                Vector3 newLocation = new Vector3(
                    MathHelper.Lerp(ownerGW.location.X, targetLocation.X, attachmentPositionMask.X),
                    MathHelper.Lerp(ownerGW.location.Y, targetLocation.Y, attachmentPositionMask.Y),
                    MathHelper.Lerp(ownerGW.location.Z, targetLocation.Z, attachmentPositionMask.Z));

                ownerGW.SetLocation(newLocation);
            }
        }
    }
}
