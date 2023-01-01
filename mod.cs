using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mod
{
    public class Mod
    {
        public static Material PinkTracer;

        public static void Main()
        {
            // registering a custom item
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Scope Attachment"), // derive from an attachment
                    NameOverride = "Bike Horn",
                    NameToOrderByOverride = "zzzzzhorn",
                    DescriptionOverride = "A bike horn that goes on the top of a gun. Does what you think it does. Attaches to the scope attachment point.",
                    CategoryOverride = ModAPI.FindCategory("Firearms"),
                    ThumbnailOverride = ModAPI.LoadSprite("horn view.png"),
                    AfterSpawn = (Instance) =>
                    {
                        // give the attachment a new sprite
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("bikehorn.png");
                        Instance.FixColliders();

                        // while you can use a new sound for the connection sound, you can do this to keep the current sound
                        // each attachment can have a unique connection sound
                        AudioClip attach = Instance.GetComponent<ScopeAttachmentBehaviour>().ConnectClip;

                        // destroy the existing attachment behaviour
                        UnityEngine.Object.Destroy(Instance.GetComponent<ScopeAttachmentBehaviour>());

                        // add the new attachment behaviour (unless it already exists)
                        var attachment = Instance.GetOrAddComponent<BikeHornAttachmentBehaviour>();

                        // set the connection sound
                        attachment.ConnectClip = attach;

                        // here is some other stuff you can change
                        attachment.AttachmentType = FirearmAttachmentType.AttachmentType.Scope; // whether the attachment will connect to the top or bottom of the gun
                        attachment.AttachmentOffset = Vector2.zero; // the offset from the attachment point (generally only used if you want the sprite to be within the gun and stuf

                        // setting the new audio clip
                        attachment.HornNoise = ModAPI.LoadSound("bikehorn.wav");
                    }
                }
            );

            // registering a custom item
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Scope Attachment"), // derive from an attachment
                    NameOverride = "Pink Round Attachment",
                    NameToOrderByOverride = "zzzzzz_im_eepy",
                    DescriptionOverride = "Somehow turns standard rounds into pink rounds. Attaches to the underbarrel attachment point.",
                    CategoryOverride = ModAPI.FindCategory("Firearms"),
                    ThumbnailOverride = ModAPI.LoadSprite("pink view.png"),
                    AfterSpawn = (Instance) =>
                    {
                        // give the attachment a new sprite
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("one_bad_gloop.png");
                        Instance.FixColliders();

                        // while you can use a new sound for the connection sound, you can do this to keep the current sound
                        // each attachment can have a unique connection sound
                        AudioClip attach = Instance.GetComponent<ScopeAttachmentBehaviour>().ConnectClip;

                        // destroy the existing attachment behaviour
                        UnityEngine.Object.Destroy(Instance.GetComponent<ScopeAttachmentBehaviour>());

                        // add the new attachment behaviour (unless it already exists)
                        var attachment = Instance.GetOrAddComponent<PinkAttachmentBehaviour>();

                        // set the connection sound
                        attachment.ConnectClip = attach;

                        // here is some other stuff you can change
                        attachment.AttachmentType = FirearmAttachmentType.AttachmentType.Barrel; // whether the attachment will connect to the top or bottom of the gun
                        attachment.AttachmentOffset = Vector2.zero; // the offset from the attachment point (generally only used if you want the sprite to be within the gun and stuf
                    }
                }
            );

            // registering a custom item
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Scope Attachment"), // derive from an attachment
                    NameOverride = "Hotdog Sight",
                    NameToOrderByOverride = "zzzzzhog",
                    DescriptionOverride = "A hotdog with a hole drilled down the middle. Improves accuracy by 100%. Attaches to the scope attachment point.",
                    CategoryOverride = ModAPI.FindCategory("Firearms"),
                    ThumbnailOverride = ModAPI.LoadSprite("hog view.png"),
                    AfterSpawn = (Instance) =>
                    {
                        // give the attachment a new sprite
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("hog.png");
                        Instance.FixColliders();

                        // we're changing the accuracy percentage of the attachment
                        // this should make the scope increase accuracy by 100%
                        Instance.GetComponent<ScopeAttachmentBehaviour>().AccuracyPercent = 100;

                        // change the connection sound if you feel like it
                        Instance.GetComponent<ScopeAttachmentBehaviour>().ConnectClip = ModAPI.LoadSound("hog.wav");
                    }
                }
            );
        }
    }

    // define the attachment behaviour
    public class BikeHornAttachmentBehaviour : FirearmAttachmentBehaviour
    {
        public AudioClip HornNoise; // The audio clip that will play on fire

        // method that is called on connection
        public override void OnConnect()
        {
        }

        // method that is called on disconnection
        public override void OnDisconnect()
        {
        }

        // method that is called when the gun is fired
        public override void OnFire()
        {
            // on fire, get the physical behaviour and run the PlayClipOnce method
            // note that you don't need to have the 'clip:' parts in the method's parameters, it makes it easier to tell what the parameters you're setting.
            PhysicalBehaviour.PlayClipOnce(clip: HornNoise, volume: 2.5f);
        }

        // method that is called when a bullet hits an object
        public override void OnHit(BallisticsEmitter.CallbackParams args)
        {
            // args contains the position of the bullet, direction of the bullet, object that was hit, and the surface normal the bullet hit.
        }
    }

    // define the attachment behaviour
    public class PinkAttachmentBehaviour : FirearmAttachmentBehaviour
    {
        // method that is called on connection
        public override void OnConnect()
        {
            FirearmBehaviour.BallisticsEmitter.OnTracerCreation.AddListener(OnTracerCreated);
        }

        // method that is called on disconnection
        public override void OnDisconnect()
        {
        }

        public void OnTracerCreated(LineRenderer line)
        {
            if (Mod.PinkTracer == null)
            {
                Mod.PinkTracer = GameObject.Instantiate<Material>(line.material);
                Mod.PinkTracer.SetColor("_Color", new Color(0.91f, 0.52f, 0.57f, 1f) * 2.5f);
            }
            line.material = Mod.PinkTracer;
        }

        // method that is called when the gun is fired
        public override void OnFire()
        {
        }

        // method that is called when a bullet hits an object
        public override void OnHit(BallisticsEmitter.CallbackParams args)
        {
            // args contains the position of the bullet, direction of the bullet, object that was hit, and the surface normal the bullet hit.
            if (args.HitObject.TryGetComponent<PhysicalBehaviour>(out var phys))
            {
                if (args.HitObject.TryGetComponent<LimbBehaviour>(out var limb))
                {
                    limb.SkinMaterialHandler.AddDamagePoint(DamageType.Burn, args.Position, FirearmBehaviour.Cartridge.Damage / 10f);
                    limb.Damage(FirearmBehaviour.Cartridge.Damage / 10f);
                }
                phys.Ignite(false);
                phys.burnIntensity = 1f;
                phys.BurnProgress += 0.1f;
            }
        }
    }
}