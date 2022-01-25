/*
 *  SquadsV
 *  Squad.cs
 *  
 *  Copyright (c) 2022 Ryan Omasta (ElementEmerald)
 * 
 * 
 *  Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 *  and associated documentation files (the "Software"), to deal in the Software without restriction,
 *  including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 *  and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
 *  subject to the following conditions:
 *
 *  The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 *
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
 *  INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 *  IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 *  WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using GTA;
using GTA.Math;
using GTA.Native;

namespace SquadsV
{
    public class Squad
    {
        private BlipColor color;
        private List<Ped> peds;
        private PedHash pedType;
        private Vehicle vehicle;
        private VehicleHash vehicleType;
        private WeaponHash weaponType1;
        private WeaponHash weaponType2;
        private RelationshipGroup group;

        private static readonly Random rand = new Random();

        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[rand.Next(s.Length)]).ToArray());
        }

        private Vector3 getRandomOffsetCoord(Vector3 start, int min, int max)
        {
            Vector3 random = new Vector3();

            if (rand.Next(0, 1) % 2 == 0)
            {
                random.X = start.X + rand.Next(min, max);
            }
            else
            {
                random.X = start.X - rand.Next(min, max);
            }

            if (rand.Next(0, 1) % 2 == 0)
            {
                random.Y = start.Y + rand.Next(min, max);
            }
            else
            {
                random.Y = start.Y - rand.Next(min, max);
            }

            return random;
        }

        public delegate void DiedEventHandler(object sender, DiedEventArgs e);

        public class DiedEventArgs
        {
            public Ped Ped { get; }
            public int Index { get; }

            public DiedEventArgs(Ped ped, int idx)
            {
                Ped = ped;
                Index = idx;
            }
        }

        public event DiedEventHandler Died;

        /// <summary>
        /// Initializes a new <see cref="Squad"/>.
        /// </summary>
        public Squad(BlipColor color, int numPeds, PedHash pedType, VehicleHash vehicleType, WeaponHash weaponType1, WeaponHash weaponType2, bool friendly = true)
        {
            // Initialize relationship group
            group = World.AddRelationshipGroup(RandomString(rand.Next(10, 25)));

            // Set relationship both ways
            group.SetRelationshipBetweenGroups(Game.Player.Character.RelationshipGroup, friendly ? Relationship.Companion : Relationship.Hate, true);

            // Set blip color
            this.color = color;

            // Initialize ped list
            peds = new List<Ped>(numPeds);

            // Set ped and vehicle models
            this.pedType = pedType;
            this.vehicleType = vehicleType;

            // Set weapon types
            this.weaponType1 = weaponType1;
            this.weaponType2 = weaponType2;
        }

        public bool Exists { get => peds.All(ped => ped.Exists()); }

        public BlipColor Color { get => color; }

        public int Count { get => peds.Count; }
		
		/// <summary>
		/// Copy of the squad peds.
		/// </summary>
		public List<Ped> Peds { get => peds; }

        public Vehicle Vehicle { get => vehicle; }

        public bool InVehicle { get => peds.All(ped => ped.Exists() && ped.IsInVehicle(vehicle)); }

        public bool LeaderReactsToEvents
        {
            set
            {
                for (int i = 0; i < peds.Capacity; i++)
                {
                    if (i == 0)
                    {
                        Ped ped = peds[i];
                        if (ped != null && ped.Exists())
                        {
                            ped.BlockPermanentEvents = !value;
                        }
                    }
                }
            }
        }

        public bool CanExitVehicle
        {
            set
            {
                foreach (Ped ped in peds)
                {
                    if (ped.Exists())
                    {
                        Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 3, value);
                    }
                }
            }
        }

        public void AttachVehicleBlip()
        {
            if (vehicle != null && vehicle.Exists() && vehicle.AttachedBlip == null)
            {
                if (vehicle.Model.IsHelicopter)
                {
                    Blip heliBlip = vehicle.AddBlip();
                    heliBlip.IsFriendly = group.GetRelationshipBetweenGroups(Game.Player.Character.RelationshipGroup) == Relationship.Companion;
                    heliBlip.Sprite = BlipSprite.HelicopterAnimated;
                    heliBlip.Color = color;
                    heliBlip.Name = "Squad Heli";
                }
                else
                {
                    Blip b = vehicle.AddBlip();
                    b.IsFriendly = group.GetRelationshipBetweenGroups(Game.Player.Character.RelationshipGroup) == Relationship.Companion;
                    b.Sprite = BlipSprite.PersonalVehicleCar;
                    b.Color = color;
                    b.Name = "Squad Vehicle";
                }
            }
        }

        public void RemoveVehicleBlip()
        {
            if (vehicle != null && vehicle.Exists() && vehicle.AttachedBlip != null) vehicle.AttachedBlip.Delete();
        }

        public void AttachPedBlips()
        {
            foreach (Ped ped in peds)
            {
                if (ped.Exists() && ped.AttachedBlip == null)
                {
                    Blip b = ped.AddBlip();
                    b.IsFriendly = ped.RelationshipGroup.GetRelationshipBetweenGroups(Game.Player.Character.RelationshipGroup) == Relationship.Companion;
                    b.Sprite = BlipSprite.VIP;
                    b.Color = color;
                    b.ScaleX = .75f;
                    b.ScaleY = .75f;
                }
            }
        }

        public void SetPedBlipSprite(Ped squadPed, BlipSprite sprite)
        {
            foreach (Ped ped in peds)
            {
                if (squadPed == ped)
                {
                    if (ped.Exists() && ped.AttachedBlip != null)
                    {
                        ped.AttachedBlip.Sprite = sprite;
                        ped.AttachedBlip.Color = color;
                    }
                }
            }
        }

        public void RemovePedBlips()
        {
            foreach (Ped ped in peds)
            {
                if (ped.Exists() && ped.AttachedBlip != null) ped.AttachedBlip.Delete();
            }
        }

        public bool Invincible
        {
            set
            {
                foreach (Ped ped in peds)
                {
                    ped.IsInvincible = value;
                }
                if (vehicle != null && vehicle.Exists() && vehicle.IsDriveable)
                {
                    vehicle.IsInvincible = value;
                }
            }
        }

        public RelationshipGroup GetRelationshipGroup()
        {
            return group;
        }

        public void ClearRelationshipWithGroup(RelationshipGroup group, Relationship r, bool bi = false)
        {
            this.group.ClearRelationshipBetweenGroups(group, r, bi);
        }

        public void SetRelationshipWithGroup(RelationshipGroup group, Relationship r, bool bi = false)
        {
            this.group.SetRelationshipBetweenGroups(group, r, bi);
        }

        /// <summary>
        /// Returns the first <see cref="Ped"/> that is in combat.
        /// </summary>
        /// <returns>First <see cref="Ped"/> that is in combat. If no peds are in combat, this returns <see langword="null"/>.</returns>
        public Ped AnyPedInCombat()
        {
            Ped result = null;

            for (int i = 0; i < peds.Count; i++)
            {
                Ped ped = peds[i];
                if (ped.Exists() && ped.IsInCombat)
                {
                    result = ped;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Checks if this <see cref="Squad"/> is dead.
        /// </summary>
        /// <returns><see langword="true"/> if this <see cref="Squad"/> is dead; otherwise <see langword="false"/>.</returns>
        public bool Dead()
        {
            bool result = true;

            for (int i = peds.Count - 1; i >= 0; i--)
            {
                Ped ped = peds[i];
                if (!ped.Exists() || Function.Call<bool>(Hash.IS_PED_DEAD_OR_DYING, ped, false))
                {
                    Died?.Invoke(this, new DiedEventArgs(ped, i + 1));
                    peds.RemoveAt(i);
                    ped.Health = 0;
                    if (ped.AttachedBlip != null && ped.AttachedBlip.Exists()) ped.AttachedBlip.Delete();
                    ped.MarkAsNoLongerNeeded();
                }
                else
                {
                    result = false;

                    /* Vector3 playerPos = Game.Player.Character.Position;
                    Vector3 enemyPos = ped.Position;

                    if (Function.Call<float>(Hash.GET_DISTANCE_BETWEEN_COORDS, playerPos.X, playerPos.Y, playerPos.Z, enemyPos.X, enemyPos.Y, enemyPos.Z, false) > 350)
                    {
                        peds.RemoveAt(i);
                        ped.Health = 0;
                        if (ped.AttachedBlip != null && ped.AttachedBlip.Exists()) ped.AttachedBlip.Delete();
                        ped.MarkAsNoLongerNeeded();
                    } */
                }
            }

            return result;
        }

        /// <summary>
        /// Spawns or respawns this <see cref="Squad"/>.
        /// </summary>
        public void Spawn()
        {
            // Check for any existing peds and mark them as no longer needed
            foreach (Ped ped in peds)
            {
                if (ped.Exists())
                {
                    ped.Health = 0;
                    if (ped.AttachedBlip != null && ped.AttachedBlip.Exists()) ped.AttachedBlip.Delete();
                    ped.MarkAsNoLongerNeeded();
                }
            }

            // Check for vehicle that exists and mark it as no longer needed
            if (vehicle != null && vehicle.Exists())
            {
                if (vehicle.AttachedBlip != null && vehicle.AttachedBlip.Exists()) vehicle.AttachedBlip.Delete();
                vehicle.MarkAsNoLongerNeeded();
            }

            // Clear ped list
            peds.Clear();

            // Load ped and vehicle models
            Model pedModel = new Model(pedType);
            Model vehicleModel = new Model(vehicleType);

            Vector3 playerPos = Game.Player.Character.Position;

            if (vehicleModel.IsHelicopter)
            {
                Vector3 spawn = getRandomOffsetCoord(playerPos, -250, -200);

                float xDiff = playerPos.X - spawn.X;
                float yDiff = playerPos.Y - spawn.Y;

                float heading = Function.Call<float>(Hash.GET_HEADING_FROM_VECTOR_2D, xDiff, yDiff);

                Vehicle heli = World.CreateVehicle(vehicleModel, new Vector3(spawn.X, spawn.Y, spawn.Z + 50), heading);
                heli.IsPersistent = true;
                heli.Mods.PrimaryColor = VehicleColor.MetallicBlack;
                heli.Mods.SecondaryColor = VehicleColor.MetallicBlack;
                heli.IsEngineRunning = true;
                heli.ForwardSpeed = 0;
                heli.EngineTorqueMultiplier = 2;

                /* Blip heliBlip = heli.AddBlip();
                heliBlip.IsFriendly = group.GetRelationshipBetweenGroups(Game.Player.Character.RelationshipGroup) == Relationship.Companion;
                heliBlip.Sprite = BlipSprite.HelicopterAnimated;
                heliBlip.Color = color;
                heliBlip.Name = "Squad Heli"; */

                vehicle = heli;
            }
            else
            {
                Vector3 spawn = new Vector3();
                int node = 0;

                OutputArgument outArg1 = new OutputArgument();
                OutputArgument outArg2 = new OutputArgument();

                if (!Function.Call<bool>(Hash.GET_RANDOM_VEHICLE_NODE, playerPos.X, playerPos.Y, playerPos.Z, 150, false, false, false, outArg1, outArg2))
                {
                    spawn = outArg1.GetResult<Vector3>();
                    node = outArg2.GetResult<int>();

                    spawn = getRandomOffsetCoord(playerPos, 50, 50);
                    OutputArgument outArg3 = new OutputArgument();

                    if (Function.Call<bool>(Hash.GET_GROUND_Z_FOR_3D_COORD, spawn.X, spawn.Y, spawn.Z, outArg3, false, false))
                    {
                        spawn.Z = outArg3.GetResult<float>();
                    }
                }

                float xDiff = playerPos.X - spawn.X;
                float yDiff = playerPos.Y - spawn.Y;

                float heading = Function.Call<float>(Hash.GET_HEADING_FROM_VECTOR_2D, xDiff, yDiff);

                Vehicle veh = World.CreateVehicle(vehicleModel, new Vector3(spawn.X, spawn.Y, spawn.Z + 5), heading);
                veh.PlaceOnGround();
                veh.IsPersistent = true;
                veh.Mods.PrimaryColor = VehicleColor.MetallicBlack;
                veh.Mods.SecondaryColor = VehicleColor.MetallicBlack;
                veh.IsEngineRunning = true;
                veh.ForwardSpeed = 0;
                veh.EngineTorqueMultiplier = 2;

                vehicle = veh;
            }

            Vector3 vehiclePos = vehicle.Position;
            DrivingStyle drivingStyle = vehicleModel.IsHelicopter ? DrivingStyle.AvoidTrafficExtremely : DrivingStyle.Rushed;

            for (int i = -1; i < (peds.Capacity - 1); i++)
            {
                Ped ped = vehicle.CreatePedOnSeat((VehicleSeat)i, pedModel);

                ped.RelationshipGroup = group;
                ped.BlockPermanentEvents = false;
                ped.IsPersistent = true;

                Function.Call(Hash.SET_PED_HEARING_RANGE, ped, 9999.0f);
                if (weaponType1 != WeaponHash.Unarmed) ped.Weapons.Give(weaponType1, 9999, true, true);
                if (weaponType2 != WeaponHash.Unarmed) ped.Weapons.Give(weaponType2, 9999, false, true);

                ped.Accuracy = 50;

                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 0, true);
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 1, true);
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 2, true);
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 3, false);
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 5, true);
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 46, true);

                /* if (!vehicleModel.IsHelicopter)
                {
                    Blip b = ped.AddBlip();
                    b.IsFriendly = group.GetRelationshipBetweenGroups(Game.Player.Character.RelationshipGroup) == Relationship.Companion;
                    b.Color = color;
                    b.ScaleX = .75f;
                    b.ScaleY = .75f;
                } */

                if (i == -1)
                {
                    if (vehicleModel.IsHelicopter)
                    {
                        Function.Call(Hash.TASK_HELI_MISSION, ped, vehicle, 0, 0, playerPos.X, playerPos.Y, playerPos.Z, 4, 75.0f, 10.0f, playerPos.ToHeading(), -1, -1, -1, 0);
                    }
                    else
                    {
                        ped.Task.DriveTo(vehicle, new Vector3(playerPos.X + 5, playerPos.Y, playerPos.Z), 20.0f, 75.0f, drivingStyle);
                    }
                }

                peds.Add(ped);
            }
        }

        /// <summary>
        /// Dismisses this <see cref="Squad"/>.
        /// </summary>
        public void Dismiss()
        {
            foreach (Ped ped in peds)
            {
                if (ped.Exists())
                {
                    ped.Delete();
                }
            }

            peds.Clear();

            if (vehicle != null && vehicle.Exists())
            {
                vehicle.Delete();
                vehicle = null;
            }

            group.Remove();
        }

        /// <summary>
        /// Makes this <see cref="Squad"/> enter or leave their vehicle.
        /// </summary>
        public void EnterLeaveVehicle()
        {
            if (vehicle != null && vehicle.Exists() && vehicle.IsDriveable)
            {
                if (InVehicle)
                {
                    foreach (Ped ped in peds)
                    {
                        ped.Task.LeaveVehicle();
                    }
                }
                else
                {
                    /*foreach (Ped ped in peds)
                    {
                        if (ped.IsInVehicle())
                        {
                            ped.Task.LeaveVehicle();
                        }

                        ped.Task.EnterVehicle(vehicle, VehicleSeat.Any, 5000, 2.0f);
                    }*/
                    for (int i = 0; i < peds.Capacity; i++)
                    {
                        Ped ped = peds[i];
                        if (ped != null && ped.Exists())
                        {
                            if (ped.IsInVehicle() && !ped.IsInVehicle(vehicle))
                            {
                                ped.Task.LeaveVehicle();
                            }

                            ped.Task.EnterVehicle(vehicle, (VehicleSeat)(i - 1), -1, 2.0f);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Makes this <see cref="Squad"/> drive to the specified <paramref name="position"/> with optional <paramref name="radius"/>, <paramref name="speed"/> and <paramref name="style"/>.
        /// </summary>
        public void DriveTo(Vector3 position, float radius = 20.0f, float speed = 50.0f, DrivingStyle style = DrivingStyle.Normal)
        {
            if (vehicle != null && vehicle.Exists() && vehicle.IsDriveable)
            {
                if (peds.All(ped => ped.Exists() && ped.IsInVehicle(vehicle)))
                {
                    Ped driver = vehicle.GetPedOnSeat(VehicleSeat.Driver);
                    if (driver != null && driver.Exists() && driver.Health > 0)
                    {
                        driver.Task.DriveTo(vehicle, position, radius, speed, style);
                    }
                }
            }
        }

        /// <summary>
        /// Makes this <see cref="Squad"/> fly to the specified <paramref name="position"/>. Squad vehicle must be a helicopter, otherwise DriveTo will be used instead.
        /// </summary>
        public void FlyTo(Vector3 position, int flag1, int flag2, float speed = 75.0f, float radius = 10.0f, DrivingStyle driveToStyle = DrivingStyle.Normal)
        {
            if (vehicle != null && vehicle.Exists() && vehicle.IsDriveable)
            {
                if (InVehicle)
                {
                    if (vehicle.Model.IsHelicopter)
                    {
                        Ped pilot = vehicle.GetPedOnSeat(VehicleSeat.Driver);

                        float xDiff = position.X - vehicle.Position.X;
                        float yDiff = position.Y - vehicle.Position.Y;

                        float heading = Function.Call<float>(Hash.GET_HEADING_FROM_VECTOR_2D, xDiff, yDiff);

                        if (pilot != null && pilot.Exists() && pilot.Health > 0)
                        {
                            Function.Call(Hash.TASK_HELI_MISSION, pilot, vehicle, 0, 0, position.X, position.Y, position.Z, flag1, speed, radius, heading, -1, -1, -1, flag2);
                        }
                    }
                    else
                    {
                        DriveTo(position, radius, speed, driveToStyle);
                    }
                }
            }
        }

        /// <summary>
        /// Makes this <see cref="Squad"/> follow the specified <paramref name="target"/>.
        /// </summary>
        public void EscortVehicle(Vehicle target, float speed = 50.0f, DrivingStyle style = DrivingStyle.Normal)
        {
            if (vehicle != null && vehicle.Exists() && vehicle.IsDriveable)
            {
                if (target != null && target.Exists() && target.IsDriveable)
                {
                    if (InVehicle)
                    {
                        Ped driver = vehicle.GetPedOnSeat(VehicleSeat.Driver);
                        if (driver != null && driver.Exists() && driver.Health > 0)
                        {
                            Function.Call(Hash.TASK_VEHICLE_ESCORT, driver, vehicle, target, -1, speed, style, 10.0f, -1, 5.0f);
                        }
                    }
                }
            }
        }

        public void FollowPlayer(bool toggle)
        {
            if (toggle)
            {
                int playerPedGroupIdx = Function.Call<int>(Hash.GET_PED_GROUP_INDEX, Game.Player.Character);

                foreach (Ped ped in peds)
                {
                    if (ped.Exists())
                    {
                        Function.Call(Hash.SET_PED_AS_GROUP_MEMBER, ped, playerPedGroupIdx);
                    }
                }
            }
            else
            {
                foreach (Ped ped in peds)
                {
                    if (ped.Exists())
                    {
                        Function.Call(Hash.REMOVE_PED_FROM_GROUP, ped);
                    }
                }
            }
        }
    }
}
