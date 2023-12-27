using System;
using BepInEx;
using GorillaLocomotion;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.UIElements;
using Utilla;
using Holdaballz.Tools;
using UnityEngine.Device;
using ComputerPlusPlus;
using Valve.VR.InteractionSystem;
using Oculus.Platform;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Primitives;
using GorillaLocomotion.Climbing;

namespace Holdaballz
{
    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        bool Enabled;
        bool InRoom;
        string HandGrabbing;
        float DistanceThingy = 0.1f;

        void Start()
        {
            Utilla.Events.GameInitialized += OnGameInitialized;
        }

        void OnEnable()
        {
            Enabled = true;

            if (InRoom)
            {
                Main.CreateBall();
            }

            HarmonyPatches.ApplyHarmonyPatches();
        }

        void OnDisable()
        {
            Enabled = false;

            HarmonyPatches.RemoveHarmonyPatches();
        }

        void OnGameInitialized(object sender, EventArgs e)
        {
            
        }

        public bool ThrowPhys;
        void Update()
        {
            if (Main.BallObj != null && Enabled && InRoom)
            {
                // Too lazy to figure out OnTrigger crap

                if (ControllerInputPoller.instance.rightGrab && GorillaLocomotion.Player.Instance.rightControllerTransform.position.Distance(Main.BallObj.transform.position) < DistanceThingy && HandGrabbing != "left")  // Right Hand
                {
                    Main.BallObj.transform.position = GorillaLocomotion.Player.Instance.rightControllerTransform.position;
                    Main.BallObj.transform.rotation = GorillaLocomotion.Player.Instance.rightControllerTransform.rotation;
                    Main.BallObj.GetComponent<Rigidbody>().useGravity = false;
                    ThrowPhys = true;
                    DistanceThingy = 10f;
                    HandGrabbing = "right";
                }
                else if (ControllerInputPoller.instance.leftGrab && GorillaLocomotion.Player.Instance.leftControllerTransform.position.Distance(Main.BallObj.transform.position) < DistanceThingy && HandGrabbing != "right") // Left hand
                {
                    Main.BallObj.transform.position = GorillaLocomotion.Player.Instance.leftControllerTransform.position;
                    Main.BallObj.transform.rotation = GorillaLocomotion.Player.Instance.leftControllerTransform.rotation;
                    Main.BallObj.GetComponent<Rigidbody>().useGravity = false;
                    ThrowPhys = true;
                    DistanceThingy = 10f;
                    HandGrabbing = "left";
                }
                else //None
                {
                    if (ThrowPhys)
                    {
                        Main.BallObj.GetComponent<Rigidbody>().velocity = Main.BallObj.GetComponent<GorillaVelocityEstimator>().linearVelocity;
                        ThrowPhys = false;
                    }
                    Main.BallObj.GetComponent<Rigidbody>().useGravity = true;
                    DistanceThingy = 0.1f;
                    HandGrabbing = "none";
                }

                if (GorillaLocomotion.Player.Instance.bodyCollider.transform.position.Distance(Main.BallObj.transform.position) > 10f) // Far from player detector
                {
                    Main.ResetBall();
                }
            }

            if (Enabled == false)
            {
                Main.DestroyBall();
            }
        }

        [ModdedGamemodeJoin]
        public void OnJoin(string gamemode)
        {
            InRoom = true;

            if (Enabled)
            {
                Main.CreateBall();
            }
        }

        [ModdedGamemodeLeave]
        public void OnLeave(string gamemode)
        {
            InRoom = false;
            Main.DestroyBall();
        }
    }
}
