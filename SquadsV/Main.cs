/*
 *  SquadsV
 *  Main.cs
 *  
 *  Copyright (c) 2021 Ryan Omasta (ElementEmerald)
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
using System.Linq;
using System.Windows.Forms;

using GTA;
using GTA.Math;

using LemonUI;
using LemonUI.Menus;

namespace SquadsV
{
    public class Main : Script
    {
        private Squad squad1;
        private Squad squad2;
        private Squad squad3;
        private Squad squad4;

        private readonly Dictionary<string, Relationship> squad1CachedRelations = new Dictionary<string, Relationship>();
        private readonly Dictionary<string, Relationship> squad2CachedRelations = new Dictionary<string, Relationship>();
        private readonly Dictionary<string, Relationship> squad3CachedRelations = new Dictionary<string, Relationship>();
        private readonly Dictionary<string, Relationship> squad4CachedRelations = new Dictionary<string, Relationship>();

        private readonly ObjectPool pool = new ObjectPool();
        private readonly NativeMenu mainMenu = new NativeMenu("SquadsV", "A remade version of Bodyguard Squads (Eddlm) by elementemerald#4175.");

        private readonly NativeMenu squad1Menu = new NativeMenu("Squad 1");
        private readonly NativeMenu squad2Menu = new NativeMenu("Squad 2");
        private readonly NativeMenu squad3Menu = new NativeMenu("Squad 3");
        private readonly NativeMenu squad4Menu = new NativeMenu("Squad 4");

        private readonly NativeMenu squad1Settings = new NativeMenu("Squad 1 Settings");
        private readonly NativeMenu squad2Settings = new NativeMenu("Squad 2 Settings");
        private readonly NativeMenu squad3Settings = new NativeMenu("Squad 3 Settings");
        private readonly NativeMenu squad4Settings = new NativeMenu("Squad 4 Settings");

        private static readonly Dictionary<string, PedHash> pedTypes = new Dictionary<string, PedHash>()
        {
            { "Mercenaries", PedHash.MerryWeatherCutscene },
            { "Ballas", PedHash.BallaEast01GMY },
            { "Families", PedHash.Famdnf01GMY }
        };

        private static readonly string[] pedTypesArray = pedTypes.Keys.ToArray();

        private readonly NativeListItem<string> squad1PedType = new NativeListItem<string>("Ped Type", pedTypesArray);
        private readonly NativeListItem<string> squad2PedType = new NativeListItem<string>("Ped Type", pedTypesArray);
        private readonly NativeListItem<string> squad3PedType = new NativeListItem<string>("Ped Type", pedTypesArray);
        private readonly NativeListItem<string> squad4PedType = new NativeListItem<string>("Ped Type", pedTypesArray);

        private readonly NativeListItem<int> squad1NumPeds = new NativeListItem<int>("# of Peds", new int[] { 1, 2, 3, 4 });
        private readonly NativeListItem<int> squad2NumPeds = new NativeListItem<int>("# of Peds", new int[] { 1, 2, 3, 4 });
        private readonly NativeListItem<int> squad3NumPeds = new NativeListItem<int>("# of Peds", new int[] { 1, 2, 3, 4 });
        private readonly NativeListItem<int> squad4NumPeds = new NativeListItem<int>("# of Peds", new int[] { 1, 2, 3, 4 });

        private static readonly Dictionary<string, VehicleHash> vehicleTypes = new Dictionary<string, VehicleHash>()
        {
            { "Buzzard", VehicleHash.Buzzard },
            { "Mesa", VehicleHash.Mesa3 }
        };

        private static readonly string[] vehicleTypesArray = vehicleTypes.Keys.ToArray();

        private readonly NativeListItem<string> squad1VehicleType = new NativeListItem<string>("Vehicle Type", vehicleTypesArray);
        private readonly NativeListItem<string> squad2VehicleType = new NativeListItem<string>("Vehicle Type", vehicleTypesArray);
        private readonly NativeListItem<string> squad3VehicleType = new NativeListItem<string>("Vehicle Type", vehicleTypesArray);
        private readonly NativeListItem<string> squad4VehicleType = new NativeListItem<string>("Vehicle Type", vehicleTypesArray);

        private static readonly Dictionary<string, WeaponHash> weaponTypes = new Dictionary<string, WeaponHash>()
        {
            { "Pistol", WeaponHash.Pistol },
            { "Micro SMG", WeaponHash.MicroSMG }
        };

        private static readonly string[] weaponTypesArray = weaponTypes.Keys.ToArray();

        private readonly NativeListItem<string> squad1MainWeapon = new NativeListItem<string>("Main Weapon", weaponTypesArray);
        private readonly NativeListItem<string> squad2MainWeapon = new NativeListItem<string>("Main Weapon", weaponTypesArray);
        private readonly NativeListItem<string> squad3MainWeapon = new NativeListItem<string>("Main Weapon", weaponTypesArray);
        private readonly NativeListItem<string> squad4MainWeapon = new NativeListItem<string>("Main Weapon", weaponTypesArray);

        private readonly NativeListItem<string> squad1SecondWeapon = new NativeListItem<string>("Secondary Weapon", weaponTypesArray);
        private readonly NativeListItem<string> squad2SecondWeapon = new NativeListItem<string>("Secondary Weapon", weaponTypesArray);
        private readonly NativeListItem<string> squad3SecondWeapon = new NativeListItem<string>("Secondary Weapon", weaponTypesArray);
        private readonly NativeListItem<string> squad4SecondWeapon = new NativeListItem<string>("Secondary Weapon", weaponTypesArray);

        private readonly NativeCheckboxItem squad1AutoRespawn = new NativeCheckboxItem("Auto Respawn", true);
        private readonly NativeCheckboxItem squad2AutoRespawn = new NativeCheckboxItem("Auto Respawn", true);
        private readonly NativeCheckboxItem squad3AutoRespawn = new NativeCheckboxItem("Auto Respawn", true);
        private readonly NativeCheckboxItem squad4AutoRespawn = new NativeCheckboxItem("Auto Respawn", true);

        private readonly NativeCheckboxItem squad1CanExitVehicle = new NativeCheckboxItem("Can Exit Vehicle", false);
        private readonly NativeCheckboxItem squad2CanExitVehicle = new NativeCheckboxItem("Can Exit Vehicle", false);
        private readonly NativeCheckboxItem squad3CanExitVehicle = new NativeCheckboxItem("Can Exit Vehicle", false);
        private readonly NativeCheckboxItem squad4CanExitVehicle = new NativeCheckboxItem("Can Exit Vehicle", false);

        private readonly NativeCheckboxItem squad1GodMode = new NativeCheckboxItem("God Mode", false);
        private readonly NativeCheckboxItem squad2GodMode = new NativeCheckboxItem("God Mode", false);
        private readonly NativeCheckboxItem squad3GodMode = new NativeCheckboxItem("God Mode", false);
        private readonly NativeCheckboxItem squad4GodMode = new NativeCheckboxItem("God Mode", false);

        private readonly NativeMenu squad1Tasks = new NativeMenu("Squad 1 Tasks");
        private readonly NativeMenu squad2Tasks = new NativeMenu("Squad 2 Tasks");
        private readonly NativeMenu squad3Tasks = new NativeMenu("Squad 3 Tasks");
        private readonly NativeMenu squad4Tasks = new NativeMenu("Squad 4 Tasks");

        private readonly NativeItem squad1EnterLeave = new NativeItem("Enter/Leave Vehicle");
        private readonly NativeItem squad2EnterLeave = new NativeItem("Enter/Leave Vehicle");
        private readonly NativeItem squad3EnterLeave = new NativeItem("Enter/Leave Vehicle");
        private readonly NativeItem squad4EnterLeave = new NativeItem("Enter/Leave Vehicle");

        private readonly NativeItem squad1DriveToWp = new NativeItem("Drive to Waypoint");
        private readonly NativeItem squad2DriveToWp = new NativeItem("Drive to Waypoint");
        private readonly NativeItem squad3DriveToWp = new NativeItem("Drive to Waypoint");
        private readonly NativeItem squad4DriveToWp = new NativeItem("Drive to Waypoint");

        private readonly NativeItem squad1EscortMe = new NativeItem("Escort my Vehicle");
        private readonly NativeItem squad2EscortMe = new NativeItem("Escort my Vehicle");
        private readonly NativeItem squad3EscortMe = new NativeItem("Escort my Vehicle");
        private readonly NativeItem squad4EscortMe = new NativeItem("Escort my Vehicle");

        private readonly NativeMenu squad1Relations = new NativeMenu("Squad 1 Relationships");
        private readonly NativeMenu squad2Relations = new NativeMenu("Squad 2 Relationships");
        private readonly NativeMenu squad3Relations = new NativeMenu("Squad 3 Relationships");
        private readonly NativeMenu squad4Relations = new NativeMenu("Squad 4 Relationships");

        private readonly NativeCheckboxItem squad1HatesPlayer = new NativeCheckboxItem("Hate Player", false);
        private readonly NativeCheckboxItem squad2HatesPlayer = new NativeCheckboxItem("Hate Player", false);
        private readonly NativeCheckboxItem squad3HatesPlayer = new NativeCheckboxItem("Hate Player", false);
        private readonly NativeCheckboxItem squad4HatesPlayer = new NativeCheckboxItem("Hate Player", false);

        private readonly NativeItem squad1RelationsHate = new NativeItem("Hate Input");
        private readonly NativeItem squad2RelationsHate = new NativeItem("Hate Input");
        private readonly NativeItem squad3RelationsHate = new NativeItem("Hate Input");
        private readonly NativeItem squad4RelationsHate = new NativeItem("Hate Input");

        private readonly NativeItem squad1Call = new NativeItem("Call Squad");
        private readonly NativeItem squad2Call = new NativeItem("Call Squad");
        private readonly NativeItem squad3Call = new NativeItem("Call Squad");
        private readonly NativeItem squad4Call = new NativeItem("Call Squad");

        private readonly NativeItem squad1Dismiss = new NativeItem("Dismiss Squad");
        private readonly NativeItem squad2Dismiss = new NativeItem("Dismiss Squad");
        private readonly NativeItem squad3Dismiss = new NativeItem("Dismiss Squad");
        private readonly NativeItem squad4Dismiss = new NativeItem("Dismiss Squad");

        public Main()
        {
            mainMenu.AddSubMenu(squad1Menu).Title = "Squad 1";
            mainMenu.AddSubMenu(squad2Menu).Title = "Squad 2";
            mainMenu.AddSubMenu(squad3Menu).Title = "Squad 3";
            mainMenu.AddSubMenu(squad4Menu).Title = "Squad 4";

            squad1Menu.AddSubMenu(squad1Settings).Title = "Squad Settings";
            squad2Menu.AddSubMenu(squad2Settings).Title = "Squad Settings";
            squad3Menu.AddSubMenu(squad3Settings).Title = "Squad Settings";
            squad4Menu.AddSubMenu(squad4Settings).Title = "Squad Settings";

            squad1Settings.Add(squad1PedType);
            squad2Settings.Add(squad2PedType);
            squad3Settings.Add(squad3PedType);
            squad4Settings.Add(squad4PedType);
            squad1Settings.Add(squad1NumPeds);
            squad2Settings.Add(squad2NumPeds);
            squad3Settings.Add(squad3NumPeds);
            squad4Settings.Add(squad4NumPeds);
            squad1Settings.Add(squad1VehicleType);
            squad2Settings.Add(squad2VehicleType);
            squad3Settings.Add(squad3VehicleType);
            squad4Settings.Add(squad4VehicleType);
            squad1Settings.Add(squad1MainWeapon);
            squad2Settings.Add(squad2MainWeapon);
            squad3Settings.Add(squad3MainWeapon);
            squad4Settings.Add(squad4MainWeapon);
            squad1Settings.Add(squad1SecondWeapon);
            squad2Settings.Add(squad2SecondWeapon);
            squad3Settings.Add(squad3SecondWeapon);
            squad4Settings.Add(squad4SecondWeapon);
            squad1Settings.Add(squad1AutoRespawn);
            squad2Settings.Add(squad2AutoRespawn);
            squad3Settings.Add(squad3AutoRespawn);
            squad4Settings.Add(squad4AutoRespawn);
            squad1Settings.Add(squad1CanExitVehicle);
            squad2Settings.Add(squad2CanExitVehicle);
            squad3Settings.Add(squad3CanExitVehicle);
            squad4Settings.Add(squad4CanExitVehicle);
            squad1Settings.Add(squad1GodMode);
            squad2Settings.Add(squad2GodMode);
            squad3Settings.Add(squad3GodMode);
            squad4Settings.Add(squad4GodMode);

            squad1Menu.AddSubMenu(squad1Tasks).Title = "Squad Tasks";
            squad2Menu.AddSubMenu(squad2Tasks).Title = "Squad Tasks";
            squad3Menu.AddSubMenu(squad3Tasks).Title = "Squad Tasks";
            squad4Menu.AddSubMenu(squad4Tasks).Title = "Squad Tasks";

            squad1Tasks.Add(squad1EnterLeave);
            squad2Tasks.Add(squad2EnterLeave);
            squad3Tasks.Add(squad3EnterLeave);
            squad4Tasks.Add(squad4EnterLeave);
            squad1Tasks.Add(squad1DriveToWp);
            squad2Tasks.Add(squad2DriveToWp);
            squad3Tasks.Add(squad3DriveToWp);
            squad4Tasks.Add(squad4DriveToWp);
            squad1Tasks.Add(squad1EscortMe);
            squad2Tasks.Add(squad2EscortMe);
            squad3Tasks.Add(squad3EscortMe);
            squad4Tasks.Add(squad4EscortMe);

            squad1Menu.AddSubMenu(squad1Relations).Title = "Squad Relationships";
            squad2Menu.AddSubMenu(squad2Relations).Title = "Squad Relationships";
            squad3Menu.AddSubMenu(squad3Relations).Title = "Squad Relationships";
            squad4Menu.AddSubMenu(squad4Relations).Title = "Squad Relationships";

            squad1Relations.Add(squad1HatesPlayer);
            squad2Relations.Add(squad2HatesPlayer);
            squad3Relations.Add(squad3HatesPlayer);
            squad4Relations.Add(squad4HatesPlayer);
            squad1Relations.Add(squad1RelationsHate);
            squad2Relations.Add(squad2RelationsHate);
            squad3Relations.Add(squad3RelationsHate);
            squad4Relations.Add(squad4RelationsHate);

            squad1Menu.Add(squad1Call);
            squad2Menu.Add(squad2Call);
            squad3Menu.Add(squad3Call);
            squad4Menu.Add(squad4Call);
            squad1Menu.Add(squad1Dismiss);
            squad2Menu.Add(squad2Dismiss);
            squad3Menu.Add(squad3Dismiss);
            squad4Menu.Add(squad4Dismiss);

            squad1Menu.ItemActivated += (s, e) =>
            {
                if (e.Item == squad1Call)
                {
                    if (squad1 == null)
                    {
                        PedHash selectedPed = PedHash.MerryWeatherCutscene;
                        VehicleHash selectedVehicle = VehicleHash.Buzzard;
                        WeaponHash selectedMainWeapon = WeaponHash.MicroSMG;
                        WeaponHash selectedSecondWeapon = WeaponHash.Pistol;

                        if (pedTypes.TryGetValue(squad1PedType.SelectedItem, out PedHash hash1)) selectedPed = hash1;
                        if (vehicleTypes.TryGetValue(squad1VehicleType.SelectedItem, out VehicleHash hash2)) selectedVehicle = hash2;
                        if (weaponTypes.TryGetValue(squad1MainWeapon.SelectedItem, out WeaponHash hash3)) selectedMainWeapon = hash3;
                        if (weaponTypes.TryGetValue(squad1SecondWeapon.SelectedItem, out WeaponHash hash4)) selectedSecondWeapon = hash4;

                        squad1 = new Squad(BlipColor.Blue, squad1NumPeds.SelectedItem, selectedPed, selectedVehicle, selectedMainWeapon, selectedSecondWeapon, !squad1HatesPlayer.Checked);
                        if (squad2 != null) squad1.SetRelationshipWithGroup(squad2.GetRelationshipGroup(), Relationship.Companion, true);
                        if (squad3 != null) squad1.SetRelationshipWithGroup(squad3.GetRelationshipGroup(), Relationship.Companion, true);
                        if (squad4 != null) squad1.SetRelationshipWithGroup(squad4.GetRelationshipGroup(), Relationship.Companion, true);
                        foreach (KeyValuePair<string, Relationship> kvp in squad1CachedRelations)
                        {
                            int hash = Game.GenerateHash(kvp.Key);
                            squad1.SetRelationshipWithGroup(new RelationshipGroup(hash), kvp.Value);
                        }
                        squad1.Spawn();
                        squad1.CanExitVehicle = squad1CanExitVehicle.Checked;
                        squad1.Invincible = squad1GodMode.Checked;
                    }
                }
                else if (e.Item == squad1Dismiss)
                {
                    if (squad1 != null)
                    {
                        squad1.Dismiss();
                        squad1 = null;
                    }
                }
            };

            squad2Menu.ItemActivated += (s, e) =>
            {
                if (e.Item == squad2Call)
                {
                    if (squad2 == null)
                    {
                        PedHash selectedPed = PedHash.MerryWeatherCutscene;
                        VehicleHash selectedVehicle = VehicleHash.Buzzard;
                        WeaponHash selectedMainWeapon = WeaponHash.MicroSMG;
                        WeaponHash selectedSecondWeapon = WeaponHash.Pistol;

                        if (pedTypes.TryGetValue(squad2PedType.SelectedItem, out PedHash hash1)) selectedPed = hash1;
                        if (vehicleTypes.TryGetValue(squad2VehicleType.SelectedItem, out VehicleHash hash2)) selectedVehicle = hash2;
                        if (weaponTypes.TryGetValue(squad2MainWeapon.SelectedItem, out WeaponHash hash3)) selectedMainWeapon = hash3;
                        if (weaponTypes.TryGetValue(squad2SecondWeapon.SelectedItem, out WeaponHash hash4)) selectedSecondWeapon = hash4;

                        squad2 = new Squad(BlipColor.Yellow, squad2NumPeds.SelectedItem, selectedPed, selectedVehicle, selectedMainWeapon, selectedSecondWeapon, !squad2HatesPlayer.Checked);
                        if (squad1 != null) squad2.SetRelationshipWithGroup(squad1.GetRelationshipGroup(), Relationship.Companion, true);
                        if (squad3 != null) squad2.SetRelationshipWithGroup(squad3.GetRelationshipGroup(), Relationship.Companion, true);
                        if (squad4 != null) squad2.SetRelationshipWithGroup(squad4.GetRelationshipGroup(), Relationship.Companion, true);
                        foreach (KeyValuePair<string, Relationship> kvp in squad2CachedRelations)
                        {
                            int hash = Game.GenerateHash(kvp.Key);
                            squad2.SetRelationshipWithGroup(new RelationshipGroup(hash), kvp.Value);
                        }
                        squad2.Spawn();
                        squad2.CanExitVehicle = squad2CanExitVehicle.Checked;
                        squad2.Invincible = squad2GodMode.Checked;
                    }
                }
                else if (e.Item == squad2Dismiss)
                {
                    if (squad2 != null)
                    {
                        squad2.Dismiss();
                        squad2 = null;
                    }
                }
            };

            squad3Menu.ItemActivated += (s, e) =>
            {
                if (e.Item == squad3Call)
                {
                    if (squad3 == null)
                    {
                        PedHash selectedPed = PedHash.MerryWeatherCutscene;
                        VehicleHash selectedVehicle = VehicleHash.Buzzard;
                        WeaponHash selectedMainWeapon = WeaponHash.MicroSMG;
                        WeaponHash selectedSecondWeapon = WeaponHash.Pistol;

                        if (pedTypes.TryGetValue(squad3PedType.SelectedItem, out PedHash hash1)) selectedPed = hash1;
                        if (vehicleTypes.TryGetValue(squad3VehicleType.SelectedItem, out VehicleHash hash2)) selectedVehicle = hash2;
                        if (weaponTypes.TryGetValue(squad3MainWeapon.SelectedItem, out WeaponHash hash3)) selectedMainWeapon = hash3;
                        if (weaponTypes.TryGetValue(squad3SecondWeapon.SelectedItem, out WeaponHash hash4)) selectedSecondWeapon = hash4;

                        squad3 = new Squad(BlipColor.Green, squad3NumPeds.SelectedItem, selectedPed, selectedVehicle, selectedMainWeapon, selectedSecondWeapon, !squad3HatesPlayer.Checked);
                        if (squad1 != null) squad3.SetRelationshipWithGroup(squad1.GetRelationshipGroup(), Relationship.Companion, true);
                        if (squad2 != null) squad3.SetRelationshipWithGroup(squad2.GetRelationshipGroup(), Relationship.Companion, true);
                        if (squad4 != null) squad3.SetRelationshipWithGroup(squad4.GetRelationshipGroup(), Relationship.Companion, true);
                        foreach (KeyValuePair<string, Relationship> kvp in squad3CachedRelations)
                        {
                            int hash = Game.GenerateHash(kvp.Key);
                            squad3.SetRelationshipWithGroup(new RelationshipGroup(hash), kvp.Value);
                        }
                        squad3.Spawn();
                        squad3.CanExitVehicle = squad3CanExitVehicle.Checked;
                        squad3.Invincible = squad3GodMode.Checked;
                    }
                }
                else if (e.Item == squad3Dismiss)
                {
                    if (squad3 != null)
                    {
                        squad3.Dismiss();
                        squad3 = null;
                    }
                }
            };

            squad4Menu.ItemActivated += (s, e) =>
            {
                if (e.Item == squad4Call)
                {
                    if (squad4 == null)
                    {
                        PedHash selectedPed = PedHash.MerryWeatherCutscene;
                        VehicleHash selectedVehicle = VehicleHash.Buzzard;
                        WeaponHash selectedMainWeapon = WeaponHash.MicroSMG;
                        WeaponHash selectedSecondWeapon = WeaponHash.Pistol;

                        if (pedTypes.TryGetValue(squad4PedType.SelectedItem, out PedHash hash1)) selectedPed = hash1;
                        if (vehicleTypes.TryGetValue(squad4VehicleType.SelectedItem, out VehicleHash hash2)) selectedVehicle = hash2;
                        if (weaponTypes.TryGetValue(squad4MainWeapon.SelectedItem, out WeaponHash hash3)) selectedMainWeapon = hash3;
                        if (weaponTypes.TryGetValue(squad4SecondWeapon.SelectedItem, out WeaponHash hash4)) selectedSecondWeapon = hash4;

                        squad4 = new Squad(BlipColor.Red, squad4NumPeds.SelectedItem, selectedPed, selectedVehicle, selectedMainWeapon, selectedSecondWeapon, !squad4HatesPlayer.Checked);
                        if (squad1 != null) squad4.SetRelationshipWithGroup(squad1.GetRelationshipGroup(), Relationship.Companion, true);
                        if (squad2 != null) squad4.SetRelationshipWithGroup(squad2.GetRelationshipGroup(), Relationship.Companion, true);
                        if (squad3 != null) squad4.SetRelationshipWithGroup(squad3.GetRelationshipGroup(), Relationship.Companion, true);
                        foreach (KeyValuePair<string, Relationship> kvp in squad4CachedRelations)
                        {
                            int hash = Game.GenerateHash(kvp.Key);
                            squad4.SetRelationshipWithGroup(new RelationshipGroup(hash), kvp.Value);
                        }
                        squad4.Spawn();
                        squad4.CanExitVehicle = squad4CanExitVehicle.Checked;
                        squad4.Invincible = squad4GodMode.Checked;
                    }
                }
                else if (e.Item == squad4Dismiss)
                {
                    if (squad4 != null)
                    {
                        squad4.Dismiss();
                        squad4 = null;
                    }
                }
            };

            squad1CanExitVehicle.CheckboxChanged += (s, e) =>
            {
                if (squad1 != null && !squad1.Dead()) squad1.CanExitVehicle = squad1CanExitVehicle.Checked;
            };

            squad2CanExitVehicle.CheckboxChanged += (s, e) =>
            {
                if (squad2 != null && !squad2.Dead()) squad2.CanExitVehicle = squad2CanExitVehicle.Checked;
            };

            squad3CanExitVehicle.CheckboxChanged += (s, e) =>
            {
                if (squad3 != null && !squad3.Dead()) squad3.CanExitVehicle = squad3CanExitVehicle.Checked;
            };

            squad4CanExitVehicle.CheckboxChanged += (s, e) =>
            {
                if (squad4 != null && !squad4.Dead()) squad4.CanExitVehicle = squad4CanExitVehicle.Checked;
            };

            squad1GodMode.CheckboxChanged += (s, e) =>
            {
                if (squad1 != null && !squad1.Dead()) squad1.Invincible = squad1GodMode.Checked;
            };

            squad2GodMode.CheckboxChanged += (s, e) =>
            {
                if (squad2 != null && !squad2.Dead()) squad2.Invincible = squad2GodMode.Checked;
            };

            squad3GodMode.CheckboxChanged += (s, e) =>
            {
                if (squad3 != null && !squad3.Dead()) squad3.Invincible = squad3GodMode.Checked;
            };

            squad4GodMode.CheckboxChanged += (s, e) =>
            {
                if (squad4 != null && !squad4.Dead()) squad4.Invincible = squad4GodMode.Checked;
            };

            squad1Tasks.ItemActivated += (s, e) =>
            {
                if (e.Item == squad1EnterLeave)
                {
                    if (squad1 != null)
                    {
                        squad1.EnterLeaveVehicle();
                    }
                }
                else if (e.Item == squad1DriveToWp)
                {
                    if (squad1 != null)
                    {
                        if (World.WaypointPosition != Vector3.Zero)
                        {
                            // If vehicle is not helicopter, they will drive instead
                            squad1.FlyTo(World.WaypointPosition, 4, 0, 75.0f, 20.0f, DrivingStyle.Rushed);
                        }
                    }
                }
                else if (e.Item == squad1EscortMe)
                {
                    squad1.EscortVehicle(Game.Player.Character.CurrentVehicle, 50.0f, DrivingStyle.Rushed);
                }
            };

            squad2Tasks.ItemActivated += (s, e) =>
            {
                if (e.Item == squad2EnterLeave)
                {
                    if (squad2 != null)
                    {
                        squad2.EnterLeaveVehicle();
                    }
                }
                else if (e.Item == squad2DriveToWp)
                {
                    if (squad2 != null)
                    {
                        if (World.WaypointPosition != Vector3.Zero)
                        {
                            // If vehicle is not helicopter, they will drive instead
                            squad2.FlyTo(World.WaypointPosition, 4, 0, 75.0f, 20.0f, DrivingStyle.Rushed);
                        }
                    }
                }
                else if (e.Item == squad2EscortMe)
                {
                    squad2.EscortVehicle(Game.Player.Character.CurrentVehicle, 50.0f, DrivingStyle.Rushed);
                }
            };

            squad3Tasks.ItemActivated += (s, e) =>
            {
                if (e.Item == squad3EnterLeave)
                {
                    if (squad3 != null)
                    {
                        squad3.EnterLeaveVehicle();
                    }
                }
                else if (e.Item == squad3DriveToWp)
                {
                    if (squad3 != null)
                    {
                        if (World.WaypointPosition != Vector3.Zero)
                        {
                            // If vehicle is not helicopter, they will drive instead
                            squad3.FlyTo(World.WaypointPosition, 4, 0, 75.0f, 20.0f, DrivingStyle.Rushed);
                        }
                    }
                }
                else if (e.Item == squad3EscortMe)
                {
                    squad3.EscortVehicle(Game.Player.Character.CurrentVehicle, 50.0f, DrivingStyle.Rushed);
                }
            };

            squad4Tasks.ItemActivated += (s, e) =>
            {
                if (e.Item == squad4EnterLeave)
                {
                    if (squad4 != null)
                    {
                        squad4.EnterLeaveVehicle();
                    }
                }
                else if (e.Item == squad4DriveToWp)
                {
                    if (squad4 != null)
                    {
                        if (World.WaypointPosition != Vector3.Zero)
                        {
                            // If vehicle is not helicopter, they will drive instead
                            squad4.FlyTo(World.WaypointPosition, 4, 0, 75.0f, 20.0f, DrivingStyle.Rushed);
                        }
                    }
                }
                else if (e.Item == squad4EscortMe)
                {
                    squad4.EscortVehicle(Game.Player.Character.CurrentVehicle, 50.0f, DrivingStyle.Rushed);
                }
            };

            squad1Relations.ItemActivated += (s, e) =>
            {
                if (e.Item == squad1RelationsHate)
                {
                    string input = Game.GetUserInput();
                    if (!string.IsNullOrEmpty(input))
                    {
                        if (squad1CachedRelations.ContainsKey(input))
                        {
                            GTA.UI.Screen.ShowSubtitle($"Squad 1 is now neutral with {input}.");
                            squad1CachedRelations.Remove(input);
                            if (squad1 != null)
                            {
                                int hash = Game.GenerateHash(input);
                                squad1.ClearRelationshipWithGroup(new RelationshipGroup(hash), Relationship.Hate);
                            }
                        }
                        else
                        {
                            GTA.UI.Screen.ShowSubtitle($"Squad 1 now hates {input}.");
                            squad1CachedRelations.Add(input, Relationship.Hate);
                            if (squad1 != null)
                            {
                                int hash = Game.GenerateHash(input);
                                squad1.SetRelationshipWithGroup(new RelationshipGroup(hash), Relationship.Hate);
                            }
                        }
                    }
                }
            };

            squad2Relations.ItemActivated += (s, e) =>
            {
                if (e.Item == squad2RelationsHate)
                {
                    string input = Game.GetUserInput();
                    if (!string.IsNullOrEmpty(input))
                    {
                        if (squad2CachedRelations.ContainsKey(input))
                        {
                            GTA.UI.Screen.ShowSubtitle($"Squad 2 is now neutral with {input}.");
                            squad2CachedRelations.Remove(input);
                            if (squad2 != null)
                            {
                                int hash = Game.GenerateHash(input);
                                squad2.ClearRelationshipWithGroup(new RelationshipGroup(hash), Relationship.Hate);
                            }
                        }
                        else
                        {
                            GTA.UI.Screen.ShowSubtitle($"Squad 2 now hates {input}.");
                            squad2CachedRelations.Add(input, Relationship.Hate);
                            if (squad2 != null)
                            {
                                int hash = Game.GenerateHash(input);
                                squad2.SetRelationshipWithGroup(new RelationshipGroup(hash), Relationship.Hate);
                            }
                        }
                    }
                }
            };

            squad3Relations.ItemActivated += (s, e) =>
            {
                if (e.Item == squad3RelationsHate)
                {
                    string input = Game.GetUserInput();
                    if (!string.IsNullOrEmpty(input))
                    {
                        if (squad3CachedRelations.ContainsKey(input))
                        {
                            GTA.UI.Screen.ShowSubtitle($"Squad 3 is now neutral with {input}.");
                            squad3CachedRelations.Remove(input);
                            if (squad3 != null)
                            {
                                int hash = Game.GenerateHash(input);
                                squad3.ClearRelationshipWithGroup(new RelationshipGroup(hash), Relationship.Hate);
                            }
                        }
                        else
                        {
                            GTA.UI.Screen.ShowSubtitle($"Squad 3 now hates {input}.");
                            squad3CachedRelations.Add(input, Relationship.Hate);
                            if (squad3 != null)
                            {
                                int hash = Game.GenerateHash(input);
                                squad3.SetRelationshipWithGroup(new RelationshipGroup(hash), Relationship.Hate);
                            }
                        }
                    }
                }
            };

            squad4Relations.ItemActivated += (s, e) =>
            {
                if (e.Item == squad4RelationsHate)
                {
                    string input = Game.GetUserInput();
                    if (!string.IsNullOrEmpty(input))
                    {
                        if (squad4CachedRelations.ContainsKey(input))
                        {
                            GTA.UI.Screen.ShowSubtitle($"Squad 4 is now neutral with {input}.");
                            squad4CachedRelations.Remove(input);
                            if (squad4 != null)
                            {
                                int hash = Game.GenerateHash(input);
                                squad4.ClearRelationshipWithGroup(new RelationshipGroup(hash), Relationship.Hate);
                            }
                        }
                        else
                        {
                            GTA.UI.Screen.ShowSubtitle($"Squad 4 now hates {input}.");
                            squad4CachedRelations.Add(input, Relationship.Hate);
                            if (squad4 != null)
                            {
                                int hash = Game.GenerateHash(input);
                                squad4.SetRelationshipWithGroup(new RelationshipGroup(hash), Relationship.Hate);
                            }
                        }
                    }
                }
            };

            squad1HatesPlayer.CheckboxChanged += (s, e) =>
            {
                if (squad1 != null) squad1.SetRelationshipWithGroup(Game.Player.Character.RelationshipGroup, squad1HatesPlayer.Checked ? Relationship.Hate : Relationship.Companion, true);
            };

            squad2HatesPlayer.CheckboxChanged += (s, e) =>
            {
                if (squad2 != null) squad2.SetRelationshipWithGroup(Game.Player.Character.RelationshipGroup, squad2HatesPlayer.Checked ? Relationship.Hate : Relationship.Companion, true);
            };

            squad3HatesPlayer.CheckboxChanged += (s, e) =>
            {
                if (squad3 != null) squad3.SetRelationshipWithGroup(Game.Player.Character.RelationshipGroup, squad3HatesPlayer.Checked ? Relationship.Hate : Relationship.Companion, true);
            };

            squad4HatesPlayer.CheckboxChanged += (s, e) =>
            {
                if (squad4 != null) squad4.SetRelationshipWithGroup(Game.Player.Character.RelationshipGroup, squad4HatesPlayer.Checked ? Relationship.Hate : Relationship.Companion, true);
            };

            pool.Add(mainMenu);

            pool.Add(squad1Menu);
            pool.Add(squad2Menu);
            pool.Add(squad3Menu);
            pool.Add(squad4Menu);

            pool.Add(squad1Settings);
            pool.Add(squad2Settings);
            pool.Add(squad3Settings);
            pool.Add(squad4Settings);

            pool.Add(squad1Tasks);
            pool.Add(squad2Tasks);
            pool.Add(squad3Tasks);
            pool.Add(squad4Tasks);

            pool.Add(squad1Relations);
            pool.Add(squad2Relations);
            pool.Add(squad3Relations);
            pool.Add(squad4Relations);

            KeyDown += onKeyDown;
            Tick += onTick;
            Aborted += onAbort;
        }

        private void onKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Z)
            {
                mainMenu.Visible = !mainMenu.Visible;
            }
        }

        private void onTick(object sender, EventArgs e)
        {
            pool.Process();

            if (squad1 != null)
            {
                // Check if all peds are in vehicle, if so then add vehicle blip
                if (squad1.Vehicle != null && squad1.Vehicle.Exists())
                {
                    if (squad1.InVehicle)
                    {
                        squad1.RemovePedBlips();
                        squad1.AttachVehicleBlip();
                    }
                    else
                    {
                        squad1.AttachPedBlips();
                        squad1.RemoveVehicleBlip();
                    }
                }

                if (squad1.Dead())
                {
                    if (squad1AutoRespawn.Checked)
                    {
                        if (squad2 != null) squad1.SetRelationshipWithGroup(squad2.GetRelationshipGroup(), Relationship.Companion, true);
                        if (squad3 != null) squad1.SetRelationshipWithGroup(squad3.GetRelationshipGroup(), Relationship.Companion, true);
                        if (squad4 != null) squad1.SetRelationshipWithGroup(squad4.GetRelationshipGroup(), Relationship.Companion, true);
                        foreach (KeyValuePair<string, Relationship> kvp in squad1CachedRelations)
                        {
                            int hash = Game.GenerateHash(kvp.Key);
                            squad1.SetRelationshipWithGroup(new RelationshipGroup(hash), kvp.Value);
                        }
                        squad1.Spawn();
                        squad1.CanExitVehicle = squad1CanExitVehicle.Checked;
                        squad1.Invincible = squad1GodMode.Checked;
                    }
                    else
                    {
                        squad1.RemovePedBlips();
                        squad1.RemoveVehicleBlip();
                        squad1 = null;
                    }
                }
            }

            if (squad2 != null)
            {
                // Check if all peds are in vehicle, if so then add vehicle blip
                if (squad2.Vehicle != null && squad2.Vehicle.Exists())
                {
                    if (squad2.InVehicle)
                    {
                        squad2.RemovePedBlips();
                        squad2.AttachVehicleBlip();
                    }
                    else
                    {
                        squad2.AttachPedBlips();
                        squad2.RemoveVehicleBlip();
                    }
                }

                if (squad2.Dead())
                {
                    if (squad2AutoRespawn.Checked)
                    {
                        if (squad1 != null) squad2.SetRelationshipWithGroup(squad1.GetRelationshipGroup(), Relationship.Companion, true);
                        if (squad3 != null) squad2.SetRelationshipWithGroup(squad3.GetRelationshipGroup(), Relationship.Companion, true);
                        if (squad4 != null) squad2.SetRelationshipWithGroup(squad4.GetRelationshipGroup(), Relationship.Companion, true);
                        foreach (KeyValuePair<string, Relationship> kvp in squad2CachedRelations)
                        {
                            int hash = Game.GenerateHash(kvp.Key);
                            squad2.SetRelationshipWithGroup(new RelationshipGroup(hash), kvp.Value);
                        }
                        squad2.Spawn();
                        squad2.CanExitVehicle = squad2CanExitVehicle.Checked;
                        squad2.Invincible = squad2GodMode.Checked;
                    }
                    else
                    {
                        squad2.RemovePedBlips();
                        squad2.RemoveVehicleBlip();
                        squad2 = null;
                    }
                }
            }

            if (squad3 != null)
            {
                // Check if all peds are in vehicle, if so then add vehicle blip
                if (squad3.Vehicle != null && squad3.Vehicle.Exists())
                {
                    if (squad3.InVehicle)
                    {
                        squad3.RemovePedBlips();
                        squad3.AttachVehicleBlip();
                    }
                    else
                    {
                        squad3.AttachPedBlips();
                        squad3.RemoveVehicleBlip();
                    }
                }

                if (squad3.Dead())
                {
                    if (squad3AutoRespawn.Checked)
                    {
                        if (squad1 != null) squad3.SetRelationshipWithGroup(squad1.GetRelationshipGroup(), Relationship.Companion, true);
                        if (squad2 != null) squad3.SetRelationshipWithGroup(squad2.GetRelationshipGroup(), Relationship.Companion, true);
                        if (squad4 != null) squad3.SetRelationshipWithGroup(squad4.GetRelationshipGroup(), Relationship.Companion, true);
                        foreach (KeyValuePair<string, Relationship> kvp in squad3CachedRelations)
                        {
                            int hash = Game.GenerateHash(kvp.Key);
                            squad3.SetRelationshipWithGroup(new RelationshipGroup(hash), kvp.Value);
                        }
                        squad3.Spawn();
                        squad3.CanExitVehicle = squad3CanExitVehicle.Checked;
                        squad3.Invincible = squad3GodMode.Checked;
                    }
                    else
                    {
                        squad3.RemovePedBlips();
                        squad3.RemoveVehicleBlip();
                        squad3 = null;
                    }
                }
            }

            if (squad4 != null)
            {
                // Check if all peds are in vehicle, if so then add vehicle blip
                if (squad4.Vehicle != null && squad4.Vehicle.Exists())
                {
                    if (squad4.InVehicle)
                    {
                        squad4.RemovePedBlips();
                        squad4.AttachVehicleBlip();
                    }
                    else
                    {
                        squad4.AttachPedBlips();
                        squad4.RemoveVehicleBlip();
                    }
                }

                if (squad4.Dead())
                {
                    if (squad4AutoRespawn.Checked)
                    {
                        if (squad1 != null) squad4.SetRelationshipWithGroup(squad1.GetRelationshipGroup(), Relationship.Companion, true);
                        if (squad2 != null) squad4.SetRelationshipWithGroup(squad2.GetRelationshipGroup(), Relationship.Companion, true);
                        if (squad3 != null) squad4.SetRelationshipWithGroup(squad3.GetRelationshipGroup(), Relationship.Companion, true);
                        foreach (KeyValuePair<string, Relationship> kvp in squad4CachedRelations)
                        {
                            int hash = Game.GenerateHash(kvp.Key);
                            squad4.SetRelationshipWithGroup(new RelationshipGroup(hash), kvp.Value);
                        }
                        squad4.Spawn();
                        squad4.CanExitVehicle = squad4CanExitVehicle.Checked;
                        squad4.Invincible = squad4GodMode.Checked;
                    }
                    else
                    {
                        squad4.RemovePedBlips();
                        squad4.RemoveVehicleBlip();
                        squad4 = null;
                    }
                }
            }
        }

        private void onAbort(object sender, EventArgs e)
        {
            if (squad1 != null)
            {
                squad1.Dismiss();
                squad1 = null;
            }
            if (squad2 != null)
            {
                squad2.Dismiss();
                squad2 = null;
            }
            if (squad3 != null)
            {
                squad3.Dismiss();
                squad3 = null;
            }
            if (squad4 != null)
            {
                squad4.Dismiss();
                squad4 = null;
            }
        }
    }
}
