/*
 *  SquadsV
 *  Main.cs
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
using System.Drawing;
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
        private Squad squad5;
        private Squad squad6;

        private readonly Dictionary<string, Relationship> squad1CachedRelations = new Dictionary<string, Relationship>();
        private readonly Dictionary<string, Relationship> squad2CachedRelations = new Dictionary<string, Relationship>();
        private readonly Dictionary<string, Relationship> squad3CachedRelations = new Dictionary<string, Relationship>();
        private readonly Dictionary<string, Relationship> squad4CachedRelations = new Dictionary<string, Relationship>();
        private readonly Dictionary<string, Relationship> squad5CachedRelations = new Dictionary<string, Relationship>();
        private readonly Dictionary<string, Relationship> squad6CachedRelations = new Dictionary<string, Relationship>();

        private readonly ObjectPool pool = new ObjectPool();
        private readonly NativeMenu mainMenu = new NativeMenu("SquadsV", "A remade version of Bodyguard Squads (Eddlm) by elementemerald#4175.");

        private readonly NativeMenu squad1Menu = new NativeMenu("Squad 1");
        private readonly NativeMenu squad2Menu = new NativeMenu("Squad 2");
        private readonly NativeMenu squad3Menu = new NativeMenu("Squad 3");
        private readonly NativeMenu squad4Menu = new NativeMenu("Squad 4");
        private readonly NativeMenu squad5Menu = new NativeMenu("Squad 5");
        private readonly NativeMenu squad6Menu = new NativeMenu("Squad 6");

        private readonly NativeMenu squad1Settings = new NativeMenu("Squad 1 Settings");
        private readonly NativeMenu squad2Settings = new NativeMenu("Squad 2 Settings");
        private readonly NativeMenu squad3Settings = new NativeMenu("Squad 3 Settings");
        private readonly NativeMenu squad4Settings = new NativeMenu("Squad 4 Settings");
        private readonly NativeMenu squad5Settings = new NativeMenu("Squad 5 Settings");
        private readonly NativeMenu squad6Settings = new NativeMenu("Squad 6 Settings");

        private static readonly Dictionary<string, PedHash[]> pedTypes = new Dictionary<string, PedHash[]>()
        {
            { "Mercenaries", new PedHash[] { PedHash.MerryWeatherCutscene } },
            { "Ballas", new PedHash[] { PedHash.BallaEast01GMY, PedHash.BallaOrig01GMY, PedHash.BallaSout01GMY } },
            { "Families", new PedHash[] { PedHash.Famdnf01GMY, PedHash.Famca01GMY, PedHash.Families01GFY } }
        };

        private static readonly string[] pedTypesArray = pedTypes.Keys.ToArray();

        private readonly NativeListItem<string> squad1PedType = new NativeListItem<string>("Ped Type", pedTypesArray);
        private readonly NativeListItem<string> squad2PedType = new NativeListItem<string>("Ped Type", pedTypesArray);
        private readonly NativeListItem<string> squad3PedType = new NativeListItem<string>("Ped Type", pedTypesArray);
        private readonly NativeListItem<string> squad4PedType = new NativeListItem<string>("Ped Type", pedTypesArray);
        private readonly NativeListItem<string> squad5PedType = new NativeListItem<string>("Ped Type", pedTypesArray);
        private readonly NativeListItem<string> squad6PedType = new NativeListItem<string>("Ped Type", pedTypesArray);

        private readonly NativeListItem<int> squad1NumPeds = new NativeListItem<int>("# of Peds", new int[] { 1, 2, 3, 4 });
        private readonly NativeListItem<int> squad2NumPeds = new NativeListItem<int>("# of Peds", new int[] { 1, 2, 3, 4 });
        private readonly NativeListItem<int> squad3NumPeds = new NativeListItem<int>("# of Peds", new int[] { 1, 2, 3, 4 });
        private readonly NativeListItem<int> squad4NumPeds = new NativeListItem<int>("# of Peds", new int[] { 1, 2, 3, 4 });
        private readonly NativeListItem<int> squad5NumPeds = new NativeListItem<int>("# of Peds", new int[] { 1, 2, 3, 4 });
        private readonly NativeListItem<int> squad6NumPeds = new NativeListItem<int>("# of Peds", new int[] { 1, 2, 3, 4 });

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
        private readonly NativeListItem<string> squad5VehicleType = new NativeListItem<string>("Vehicle Type", vehicleTypesArray);
        private readonly NativeListItem<string> squad6VehicleType = new NativeListItem<string>("Vehicle Type", vehicleTypesArray);

        private static readonly Dictionary<string, WeaponHash> primaryWeaponTypes = new Dictionary<string, WeaponHash>()
        {
            { "Unarmed", WeaponHash.Unarmed },
            { "Pistol", WeaponHash.Pistol },
            { "Micro SMG", WeaponHash.MicroSMG },
            { "Assault Rifle", WeaponHash.AssaultRifle },
            { "RPG", WeaponHash.RPG }
        };

        private static readonly Dictionary<string, WeaponHash> secondaryWeaponTypes = new Dictionary<string, WeaponHash>()
        {
            { "Unarmed", WeaponHash.Unarmed },
            { "Pistol", WeaponHash.Pistol },
            { "Micro SMG", WeaponHash.MicroSMG }
        };

        private static readonly string[] primaryWeaponTypesArray = primaryWeaponTypes.Keys.ToArray();
        private static readonly string[] secondaryWeaponTypesArray = secondaryWeaponTypes.Keys.ToArray();

        private readonly NativeListItem<string> squad1MainWeapon = new NativeListItem<string>("Main Weapon", primaryWeaponTypesArray);
        private readonly NativeListItem<string> squad2MainWeapon = new NativeListItem<string>("Main Weapon", primaryWeaponTypesArray);
        private readonly NativeListItem<string> squad3MainWeapon = new NativeListItem<string>("Main Weapon", primaryWeaponTypesArray);
        private readonly NativeListItem<string> squad4MainWeapon = new NativeListItem<string>("Main Weapon", primaryWeaponTypesArray);
        private readonly NativeListItem<string> squad5MainWeapon = new NativeListItem<string>("Main Weapon", primaryWeaponTypesArray);
        private readonly NativeListItem<string> squad6MainWeapon = new NativeListItem<string>("Main Weapon", primaryWeaponTypesArray);

        private readonly NativeListItem<string> squad1SecondWeapon = new NativeListItem<string>("Secondary Weapon", secondaryWeaponTypesArray);
        private readonly NativeListItem<string> squad2SecondWeapon = new NativeListItem<string>("Secondary Weapon", secondaryWeaponTypesArray);
        private readonly NativeListItem<string> squad3SecondWeapon = new NativeListItem<string>("Secondary Weapon", secondaryWeaponTypesArray);
        private readonly NativeListItem<string> squad4SecondWeapon = new NativeListItem<string>("Secondary Weapon", secondaryWeaponTypesArray);
        private readonly NativeListItem<string> squad5SecondWeapon = new NativeListItem<string>("Secondary Weapon", secondaryWeaponTypesArray);
        private readonly NativeListItem<string> squad6SecondWeapon = new NativeListItem<string>("Secondary Weapon", secondaryWeaponTypesArray);

        private readonly NativeCheckboxItem squad1AutoRespawn = new NativeCheckboxItem("Auto Respawn", true);
        private readonly NativeCheckboxItem squad2AutoRespawn = new NativeCheckboxItem("Auto Respawn", true);
        private readonly NativeCheckboxItem squad3AutoRespawn = new NativeCheckboxItem("Auto Respawn", true);
        private readonly NativeCheckboxItem squad4AutoRespawn = new NativeCheckboxItem("Auto Respawn", true);
        private readonly NativeCheckboxItem squad5AutoRespawn = new NativeCheckboxItem("Auto Respawn", true);
        private readonly NativeCheckboxItem squad6AutoRespawn = new NativeCheckboxItem("Auto Respawn", true);

        private readonly NativeCheckboxItem squad1CanExitVehicle = new NativeCheckboxItem("Can Exit Vehicle", false);
        private readonly NativeCheckboxItem squad2CanExitVehicle = new NativeCheckboxItem("Can Exit Vehicle", false);
        private readonly NativeCheckboxItem squad3CanExitVehicle = new NativeCheckboxItem("Can Exit Vehicle", false);
        private readonly NativeCheckboxItem squad4CanExitVehicle = new NativeCheckboxItem("Can Exit Vehicle", false);
        private readonly NativeCheckboxItem squad5CanExitVehicle = new NativeCheckboxItem("Can Exit Vehicle", false);
        private readonly NativeCheckboxItem squad6CanExitVehicle = new NativeCheckboxItem("Can Exit Vehicle", false);

        private readonly NativeCheckboxItem squad1LeaderReacts = new NativeCheckboxItem("Leader Reacts to Events", true);
        private readonly NativeCheckboxItem squad2LeaderReacts = new NativeCheckboxItem("Leader Reacts to Events", true);
        private readonly NativeCheckboxItem squad3LeaderReacts = new NativeCheckboxItem("Leader Reacts to Events", true);
        private readonly NativeCheckboxItem squad4LeaderReacts = new NativeCheckboxItem("Leader Reacts to Events", true);
        private readonly NativeCheckboxItem squad5LeaderReacts = new NativeCheckboxItem("Leader Reacts to Events", true);
        private readonly NativeCheckboxItem squad6LeaderReacts = new NativeCheckboxItem("Leader Reacts to Events", true);

        private readonly NativeCheckboxItem squad1GodMode = new NativeCheckboxItem("God Mode", false);
        private readonly NativeCheckboxItem squad2GodMode = new NativeCheckboxItem("God Mode", false);
        private readonly NativeCheckboxItem squad3GodMode = new NativeCheckboxItem("God Mode", false);
        private readonly NativeCheckboxItem squad4GodMode = new NativeCheckboxItem("God Mode", false);
        private readonly NativeCheckboxItem squad5GodMode = new NativeCheckboxItem("God Mode", false);
        private readonly NativeCheckboxItem squad6GodMode = new NativeCheckboxItem("God Mode", false);

        private readonly NativeMenu squad1Tasks = new NativeMenu("Squad 1 Tasks");
        private readonly NativeMenu squad2Tasks = new NativeMenu("Squad 2 Tasks");
        private readonly NativeMenu squad3Tasks = new NativeMenu("Squad 3 Tasks");
        private readonly NativeMenu squad4Tasks = new NativeMenu("Squad 4 Tasks");
        private readonly NativeMenu squad5Tasks = new NativeMenu("Squad 5 Tasks");
        private readonly NativeMenu squad6Tasks = new NativeMenu("Squad 6 Tasks");

        private readonly NativeCheckboxItem squad1FollowPlayer = new NativeCheckboxItem("Follow Player", false);
        private readonly NativeCheckboxItem squad2FollowPlayer = new NativeCheckboxItem("Follow Player", false);
        private readonly NativeCheckboxItem squad3FollowPlayer = new NativeCheckboxItem("Follow Player", false);
        private readonly NativeCheckboxItem squad4FollowPlayer = new NativeCheckboxItem("Follow Player", false);
        private readonly NativeCheckboxItem squad5FollowPlayer = new NativeCheckboxItem("Follow Player", false);
        private readonly NativeCheckboxItem squad6FollowPlayer = new NativeCheckboxItem("Follow Player", false);

        private readonly NativeItem squad1EnterLeave = new NativeItem("Enter/Leave Vehicle");
        private readonly NativeItem squad2EnterLeave = new NativeItem("Enter/Leave Vehicle");
        private readonly NativeItem squad3EnterLeave = new NativeItem("Enter/Leave Vehicle");
        private readonly NativeItem squad4EnterLeave = new NativeItem("Enter/Leave Vehicle");
        private readonly NativeItem squad5EnterLeave = new NativeItem("Enter/Leave Vehicle");
        private readonly NativeItem squad6EnterLeave = new NativeItem("Enter/Leave Vehicle");

        private readonly NativeItem squad1DriveToWp = new NativeItem("Drive to Waypoint");
        private readonly NativeItem squad2DriveToWp = new NativeItem("Drive to Waypoint");
        private readonly NativeItem squad3DriveToWp = new NativeItem("Drive to Waypoint");
        private readonly NativeItem squad4DriveToWp = new NativeItem("Drive to Waypoint");
        private readonly NativeItem squad5DriveToWp = new NativeItem("Drive to Waypoint");
        private readonly NativeItem squad6DriveToWp = new NativeItem("Drive to Waypoint");

        private readonly NativeItem squad1EscortMe = new NativeItem("Escort my Vehicle");
        private readonly NativeItem squad2EscortMe = new NativeItem("Escort my Vehicle");
        private readonly NativeItem squad3EscortMe = new NativeItem("Escort my Vehicle");
        private readonly NativeItem squad4EscortMe = new NativeItem("Escort my Vehicle");
        private readonly NativeItem squad5EscortMe = new NativeItem("Escort my Vehicle");
        private readonly NativeItem squad6EscortMe = new NativeItem("Escort my Vehicle");

        private readonly NativeItem squad1RunToMe = new NativeItem("Get back to me");
        private readonly NativeItem squad2RunToMe = new NativeItem("Get back to me");
        private readonly NativeItem squad3RunToMe = new NativeItem("Get back to me");
        private readonly NativeItem squad4RunToMe = new NativeItem("Get back to me");
        private readonly NativeItem squad5RunToMe = new NativeItem("Get back to me");
        private readonly NativeItem squad6RunToMe = new NativeItem("Get back to me");

        private readonly NativeMenu squad1Relations = new NativeMenu("Squad 1 Relationships");
        private readonly NativeMenu squad2Relations = new NativeMenu("Squad 2 Relationships");
        private readonly NativeMenu squad3Relations = new NativeMenu("Squad 3 Relationships");
        private readonly NativeMenu squad4Relations = new NativeMenu("Squad 4 Relationships");
        private readonly NativeMenu squad5Relations = new NativeMenu("Squad 5 Relationships");
        private readonly NativeMenu squad6Relations = new NativeMenu("Squad 6 Relationships");

        private readonly NativeCheckboxItem squad1HatesPlayer = new NativeCheckboxItem("Hate Player", false);
        private readonly NativeCheckboxItem squad2HatesPlayer = new NativeCheckboxItem("Hate Player", false);
        private readonly NativeCheckboxItem squad3HatesPlayer = new NativeCheckboxItem("Hate Player", false);
        private readonly NativeCheckboxItem squad4HatesPlayer = new NativeCheckboxItem("Hate Player", false);
        private readonly NativeCheckboxItem squad5HatesPlayer = new NativeCheckboxItem("Hate Player", false);
        private readonly NativeCheckboxItem squad6HatesPlayer = new NativeCheckboxItem("Hate Player", false);

        // Squad 1
        private readonly NativeCheckboxItem squad1HatesSquad2 = new NativeCheckboxItem("Hate Squad 2", false);
        private readonly NativeCheckboxItem squad1HatesSquad3 = new NativeCheckboxItem("Hate Squad 3", false);
        private readonly NativeCheckboxItem squad1HatesSquad4 = new NativeCheckboxItem("Hate Squad 4", false);
        private readonly NativeCheckboxItem squad1HatesSquad5 = new NativeCheckboxItem("Hate Squad 5", false);
        private readonly NativeCheckboxItem squad1HatesSquad6 = new NativeCheckboxItem("Hate Squad 6", false);

        // Squad 2
        private readonly NativeCheckboxItem squad2HatesSquad1 = new NativeCheckboxItem("Hate Squad 1", false);
        private readonly NativeCheckboxItem squad2HatesSquad3 = new NativeCheckboxItem("Hate Squad 3", false);
        private readonly NativeCheckboxItem squad2HatesSquad4 = new NativeCheckboxItem("Hate Squad 4", false);
        private readonly NativeCheckboxItem squad2HatesSquad5 = new NativeCheckboxItem("Hate Squad 5", false);
        private readonly NativeCheckboxItem squad2HatesSquad6 = new NativeCheckboxItem("Hate Squad 6", false);

        // Squad 3
        private readonly NativeCheckboxItem squad3HatesSquad1 = new NativeCheckboxItem("Hate Squad 1", false);
        private readonly NativeCheckboxItem squad3HatesSquad2 = new NativeCheckboxItem("Hate Squad 2", false);
        private readonly NativeCheckboxItem squad3HatesSquad4 = new NativeCheckboxItem("Hate Squad 4", false);
        private readonly NativeCheckboxItem squad3HatesSquad5 = new NativeCheckboxItem("Hate Squad 5", false);
        private readonly NativeCheckboxItem squad3HatesSquad6 = new NativeCheckboxItem("Hate Squad 6", false);

        // Squad 4
        private readonly NativeCheckboxItem squad4HatesSquad1 = new NativeCheckboxItem("Hate Squad 1", false);
        private readonly NativeCheckboxItem squad4HatesSquad2 = new NativeCheckboxItem("Hate Squad 2", false);
        private readonly NativeCheckboxItem squad4HatesSquad3 = new NativeCheckboxItem("Hate Squad 3", false);
        private readonly NativeCheckboxItem squad4HatesSquad5 = new NativeCheckboxItem("Hate Squad 5", false);
        private readonly NativeCheckboxItem squad4HatesSquad6 = new NativeCheckboxItem("Hate Squad 6", false);

        // Squad 5
        private readonly NativeCheckboxItem squad5HatesSquad1 = new NativeCheckboxItem("Hate Squad 1", false);
        private readonly NativeCheckboxItem squad5HatesSquad2 = new NativeCheckboxItem("Hate Squad 2", false);
        private readonly NativeCheckboxItem squad5HatesSquad3 = new NativeCheckboxItem("Hate Squad 3", false);
        private readonly NativeCheckboxItem squad5HatesSquad4 = new NativeCheckboxItem("Hate Squad 4", false);
        private readonly NativeCheckboxItem squad5HatesSquad6 = new NativeCheckboxItem("Hate Squad 6", false);

        // Squad 6
        private readonly NativeCheckboxItem squad6HatesSquad1 = new NativeCheckboxItem("Hate Squad 1", false);
        private readonly NativeCheckboxItem squad6HatesSquad2 = new NativeCheckboxItem("Hate Squad 2", false);
        private readonly NativeCheckboxItem squad6HatesSquad3 = new NativeCheckboxItem("Hate Squad 3", false);
        private readonly NativeCheckboxItem squad6HatesSquad4 = new NativeCheckboxItem("Hate Squad 4", false);
        private readonly NativeCheckboxItem squad6HatesSquad5 = new NativeCheckboxItem("Hate Squad 5", false);

        private readonly NativeItem squad1RelationsHate = new NativeItem("Hate Input");
        private readonly NativeItem squad2RelationsHate = new NativeItem("Hate Input");
        private readonly NativeItem squad3RelationsHate = new NativeItem("Hate Input");
        private readonly NativeItem squad4RelationsHate = new NativeItem("Hate Input");
        private readonly NativeItem squad5RelationsHate = new NativeItem("Hate Input");
        private readonly NativeItem squad6RelationsHate = new NativeItem("Hate Input");

        private readonly NativeItem squad1Call = new NativeItem("Call Squad");
        private readonly NativeItem squad2Call = new NativeItem("Call Squad");
        private readonly NativeItem squad3Call = new NativeItem("Call Squad");
        private readonly NativeItem squad4Call = new NativeItem("Call Squad");
        private readonly NativeItem squad5Call = new NativeItem("Call Squad");
        private readonly NativeItem squad6Call = new NativeItem("Call Squad");

        private readonly NativeItem squad1Dismiss = new NativeItem("Dismiss Squad");
        private readonly NativeItem squad2Dismiss = new NativeItem("Dismiss Squad");
        private readonly NativeItem squad3Dismiss = new NativeItem("Dismiss Squad");
        private readonly NativeItem squad4Dismiss = new NativeItem("Dismiss Squad");
        private readonly NativeItem squad5Dismiss = new NativeItem("Dismiss Squad");
        private readonly NativeItem squad6Dismiss = new NativeItem("Dismiss Squad");

        public Main()
        {
            GTA.UI.Notification.Show(string.Format("SquadsV v{0} has loaded.", typeof(Main).Assembly.GetName().Version));

            mainMenu.AddSubMenu(squad1Menu).Title = "Squad 1";
            mainMenu.AddSubMenu(squad2Menu).Title = "Squad 2";
            mainMenu.AddSubMenu(squad3Menu).Title = "Squad 3";
            mainMenu.AddSubMenu(squad4Menu).Title = "Squad 4";
            mainMenu.AddSubMenu(squad5Menu).Title = "Squad 5";
            mainMenu.AddSubMenu(squad6Menu).Title = "Squad 6";

            squad1Menu.AddSubMenu(squad1Settings).Title = "Squad Settings";
            squad2Menu.AddSubMenu(squad2Settings).Title = "Squad Settings";
            squad3Menu.AddSubMenu(squad3Settings).Title = "Squad Settings";
            squad4Menu.AddSubMenu(squad4Settings).Title = "Squad Settings";
            squad5Menu.AddSubMenu(squad5Settings).Title = "Squad Settings";
            squad6Menu.AddSubMenu(squad6Settings).Title = "Squad Settings";

            // Ped Type
            squad1Settings.Add(squad1PedType);
            squad2Settings.Add(squad2PedType);
            squad3Settings.Add(squad3PedType);
            squad4Settings.Add(squad4PedType);
            squad5Settings.Add(squad5PedType);
            squad6Settings.Add(squad6PedType);
            // # of Peds
            squad1Settings.Add(squad1NumPeds);
            squad2Settings.Add(squad2NumPeds);
            squad3Settings.Add(squad3NumPeds);
            squad4Settings.Add(squad4NumPeds);
            squad5Settings.Add(squad5NumPeds);
            squad6Settings.Add(squad6NumPeds);
            // Vehicle Type
            squad1Settings.Add(squad1VehicleType);
            squad2Settings.Add(squad2VehicleType);
            squad3Settings.Add(squad3VehicleType);
            squad4Settings.Add(squad4VehicleType);
            squad5Settings.Add(squad5VehicleType);
            squad6Settings.Add(squad6VehicleType);
            // Main Weapon
            squad1Settings.Add(squad1MainWeapon);
            squad2Settings.Add(squad2MainWeapon);
            squad3Settings.Add(squad3MainWeapon);
            squad4Settings.Add(squad4MainWeapon);
            squad5Settings.Add(squad5MainWeapon);
            squad6Settings.Add(squad6MainWeapon);
            // Secondary Weapon
            squad1Settings.Add(squad1SecondWeapon);
            squad2Settings.Add(squad2SecondWeapon);
            squad3Settings.Add(squad3SecondWeapon);
            squad4Settings.Add(squad4SecondWeapon);
            squad5Settings.Add(squad5SecondWeapon);
            squad6Settings.Add(squad6SecondWeapon);
            // Auto Respawn
            squad1Settings.Add(squad1AutoRespawn);
            squad2Settings.Add(squad2AutoRespawn);
            squad3Settings.Add(squad3AutoRespawn);
            squad4Settings.Add(squad4AutoRespawn);
            squad5Settings.Add(squad5AutoRespawn);
            squad6Settings.Add(squad6AutoRespawn);
            // Can Exit Vehicle
            squad1Settings.Add(squad1CanExitVehicle);
            squad2Settings.Add(squad2CanExitVehicle);
            squad3Settings.Add(squad3CanExitVehicle);
            squad4Settings.Add(squad4CanExitVehicle);
            squad5Settings.Add(squad5CanExitVehicle);
            squad6Settings.Add(squad6CanExitVehicle);
            // Leader Reacts to Events
            squad1Settings.Add(squad1LeaderReacts);
            squad2Settings.Add(squad2LeaderReacts);
            squad3Settings.Add(squad3LeaderReacts);
            squad4Settings.Add(squad4LeaderReacts);
            squad5Settings.Add(squad5LeaderReacts);
            squad6Settings.Add(squad6LeaderReacts);
            // God Mode
            squad1Settings.Add(squad1GodMode);
            squad2Settings.Add(squad2GodMode);
            squad3Settings.Add(squad3GodMode);
            squad4Settings.Add(squad4GodMode);
            squad5Settings.Add(squad5GodMode);
            squad6Settings.Add(squad6GodMode);

            squad1Menu.AddSubMenu(squad1Tasks).Title = "Squad Tasks";
            squad2Menu.AddSubMenu(squad2Tasks).Title = "Squad Tasks";
            squad3Menu.AddSubMenu(squad3Tasks).Title = "Squad Tasks";
            squad4Menu.AddSubMenu(squad4Tasks).Title = "Squad Tasks";
            squad5Menu.AddSubMenu(squad5Tasks).Title = "Squad Tasks";
            squad6Menu.AddSubMenu(squad6Tasks).Title = "Squad Tasks";

            // Follow Player
            squad1Tasks.Add(squad1FollowPlayer);
            squad2Tasks.Add(squad2FollowPlayer);
            squad3Tasks.Add(squad3FollowPlayer);
            squad4Tasks.Add(squad4FollowPlayer);
            squad5Tasks.Add(squad5FollowPlayer);
            squad6Tasks.Add(squad6FollowPlayer);
            // Enter/Leave Vehicle
            squad1Tasks.Add(squad1EnterLeave);
            squad2Tasks.Add(squad2EnterLeave);
            squad3Tasks.Add(squad3EnterLeave);
            squad4Tasks.Add(squad4EnterLeave);
            squad5Tasks.Add(squad5EnterLeave);
            squad6Tasks.Add(squad6EnterLeave);
            // Drive to Waypoint
            squad1Tasks.Add(squad1DriveToWp);
            squad2Tasks.Add(squad2DriveToWp);
            squad3Tasks.Add(squad3DriveToWp);
            squad4Tasks.Add(squad4DriveToWp);
            squad5Tasks.Add(squad5DriveToWp);
            squad6Tasks.Add(squad6DriveToWp);
            // Escort Vehicle
            squad1Tasks.Add(squad1EscortMe);
            squad2Tasks.Add(squad2EscortMe);
            squad3Tasks.Add(squad3EscortMe);
            squad4Tasks.Add(squad4EscortMe);
            squad5Tasks.Add(squad5EscortMe);
            squad6Tasks.Add(squad6EscortMe);
            // Get back to me
            squad1Tasks.Add(squad1RunToMe);
            squad2Tasks.Add(squad2RunToMe);
            squad3Tasks.Add(squad3RunToMe);
            squad4Tasks.Add(squad4RunToMe);
            squad5Tasks.Add(squad5RunToMe);
            squad6Tasks.Add(squad6RunToMe);

            squad1Menu.AddSubMenu(squad1Relations).Title = "Squad Relationships";
            squad2Menu.AddSubMenu(squad2Relations).Title = "Squad Relationships";
            squad3Menu.AddSubMenu(squad3Relations).Title = "Squad Relationships";
            squad4Menu.AddSubMenu(squad4Relations).Title = "Squad Relationships";
            squad5Menu.AddSubMenu(squad5Relations).Title = "Squad Relationships";
            squad6Menu.AddSubMenu(squad6Relations).Title = "Squad Relationships";

            squad1Relations.Add(squad1HatesPlayer);
            squad2Relations.Add(squad2HatesPlayer);
            squad3Relations.Add(squad3HatesPlayer);
            squad4Relations.Add(squad4HatesPlayer);
            squad5Relations.Add(squad5HatesPlayer);
            squad6Relations.Add(squad6HatesPlayer);

            // Squad 1
            squad1Relations.Add(squad1HatesSquad2);
            squad1Relations.Add(squad1HatesSquad3);
            squad1Relations.Add(squad1HatesSquad4);
            squad1Relations.Add(squad1HatesSquad5);
            squad1Relations.Add(squad1HatesSquad6);

            // Squad 2
            squad2Relations.Add(squad2HatesSquad1);
            squad2Relations.Add(squad2HatesSquad3);
            squad2Relations.Add(squad2HatesSquad4);
            squad2Relations.Add(squad2HatesSquad5);
            squad2Relations.Add(squad2HatesSquad6);

            // Squad 3
            squad3Relations.Add(squad3HatesSquad1);
            squad3Relations.Add(squad3HatesSquad2);
            squad3Relations.Add(squad3HatesSquad4);
            squad3Relations.Add(squad3HatesSquad5);
            squad3Relations.Add(squad3HatesSquad6);

            // Squad 4
            squad4Relations.Add(squad4HatesSquad1);
            squad4Relations.Add(squad4HatesSquad2);
            squad4Relations.Add(squad4HatesSquad3);
            squad4Relations.Add(squad4HatesSquad5);
            squad4Relations.Add(squad4HatesSquad6);

            // Squad 5
            squad5Relations.Add(squad5HatesSquad1);
            squad5Relations.Add(squad5HatesSquad2);
            squad5Relations.Add(squad5HatesSquad3);
            squad5Relations.Add(squad5HatesSquad4);
            squad5Relations.Add(squad5HatesSquad6);

            // Squad 6
            squad6Relations.Add(squad6HatesSquad1);
            squad6Relations.Add(squad6HatesSquad2);
            squad6Relations.Add(squad6HatesSquad3);
            squad6Relations.Add(squad6HatesSquad4);
            squad6Relations.Add(squad6HatesSquad5);

            squad1Relations.Add(squad1RelationsHate);
            squad2Relations.Add(squad2RelationsHate);
            squad3Relations.Add(squad3RelationsHate);
            squad4Relations.Add(squad4RelationsHate);
            squad5Relations.Add(squad5RelationsHate);
            squad6Relations.Add(squad6RelationsHate);

            // Call
            squad1Menu.Add(squad1Call);
            squad2Menu.Add(squad2Call);
            squad3Menu.Add(squad3Call);
            squad4Menu.Add(squad4Call);
            squad5Menu.Add(squad5Call);
            squad6Menu.Add(squad6Call);
            // Dismiss
            squad1Menu.Add(squad1Dismiss);
            squad2Menu.Add(squad2Dismiss);
            squad3Menu.Add(squad3Dismiss);
            squad4Menu.Add(squad4Dismiss);
            squad5Menu.Add(squad5Dismiss);
            squad6Menu.Add(squad6Dismiss);

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

                        if (pedTypes.TryGetValue(squad1PedType.SelectedItem, out PedHash[] modelHashes))
                        {
                            Random rand = new Random();
                            selectedPed = modelHashes[rand.Next(1, modelHashes.Length)];
                        }

                        if (vehicleTypes.TryGetValue(squad1VehicleType.SelectedItem, out VehicleHash hash2)) selectedVehicle = hash2;
                        if (primaryWeaponTypes.TryGetValue(squad1MainWeapon.SelectedItem, out WeaponHash hash3)) selectedMainWeapon = hash3;
                        if (secondaryWeaponTypes.TryGetValue(squad1SecondWeapon.SelectedItem, out WeaponHash hash4)) selectedSecondWeapon = hash4;

                        squad1 = new Squad(BlipColor.Blue, squad1NumPeds.SelectedItem, selectedPed, selectedVehicle, selectedMainWeapon, selectedSecondWeapon, !squad1HatesPlayer.Checked);
                        if (squad2 != null) squad1.SetRelationshipWithGroup(squad2.GetRelationshipGroup(), squad1HatesSquad2.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad3 != null) squad1.SetRelationshipWithGroup(squad3.GetRelationshipGroup(), squad1HatesSquad3.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad4 != null) squad1.SetRelationshipWithGroup(squad4.GetRelationshipGroup(), squad1HatesSquad4.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad5 != null) squad1.SetRelationshipWithGroup(squad5.GetRelationshipGroup(), squad1HatesSquad5.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad6 != null) squad1.SetRelationshipWithGroup(squad6.GetRelationshipGroup(), squad1HatesSquad6.Checked ? Relationship.Hate : Relationship.Companion, true);
                        foreach (KeyValuePair<string, Relationship> kvp in squad1CachedRelations)
                        {
                            int hash = Game.GenerateHash(kvp.Key);
                            squad1.SetRelationshipWithGroup(new RelationshipGroup(hash), kvp.Value);
                        }
                        squad1.Spawn();
                        squad1.CanExitVehicle = squad1CanExitVehicle.Checked;
                        squad1.LeaderReactsToEvents = squad1LeaderReacts.Checked;
                        squad1.Invincible = squad1GodMode.Checked;

                        squad1.Died += (sender, args) =>
                        {
                            if (args.Ped.Exists() && args.Ped.Killer.Exists())
                            {
                                GTA.UI.Notification.Show(string.Format("Squad 1 member {0} killed by {1}", args.Index, args.Ped.Killer.Model.ToString()));
                            }
                        };
                    }
                }
                else if (e.Item == squad1Dismiss)
                {
                    if (squad1 != null)
                    {
                        squad1.Dismiss();
                        squad1 = null;
                        squad1FollowPlayer.Checked = false;
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

                        if (pedTypes.TryGetValue(squad2PedType.SelectedItem, out PedHash[] modelHashes))
                        {
                            Random rand = new Random();
                            selectedPed = modelHashes[rand.Next(1, modelHashes.Length)];
                        }

                        if (vehicleTypes.TryGetValue(squad2VehicleType.SelectedItem, out VehicleHash hash2)) selectedVehicle = hash2;
                        if (primaryWeaponTypes.TryGetValue(squad2MainWeapon.SelectedItem, out WeaponHash hash3)) selectedMainWeapon = hash3;
                        if (secondaryWeaponTypes.TryGetValue(squad2SecondWeapon.SelectedItem, out WeaponHash hash4)) selectedSecondWeapon = hash4;

                        squad2 = new Squad(BlipColor.Yellow, squad2NumPeds.SelectedItem, selectedPed, selectedVehicle, selectedMainWeapon, selectedSecondWeapon, !squad2HatesPlayer.Checked);
                        if (squad1 != null) squad2.SetRelationshipWithGroup(squad1.GetRelationshipGroup(), squad2HatesSquad1.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad3 != null) squad2.SetRelationshipWithGroup(squad3.GetRelationshipGroup(), squad2HatesSquad3.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad4 != null) squad2.SetRelationshipWithGroup(squad4.GetRelationshipGroup(), squad2HatesSquad4.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad5 != null) squad2.SetRelationshipWithGroup(squad5.GetRelationshipGroup(), squad2HatesSquad5.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad6 != null) squad2.SetRelationshipWithGroup(squad6.GetRelationshipGroup(), squad2HatesSquad6.Checked ? Relationship.Hate : Relationship.Companion, true);
                        foreach (KeyValuePair<string, Relationship> kvp in squad2CachedRelations)
                        {
                            int hash = Game.GenerateHash(kvp.Key);
                            squad2.SetRelationshipWithGroup(new RelationshipGroup(hash), kvp.Value);
                        }
                        squad2.Spawn();
                        squad2.CanExitVehicle = squad2CanExitVehicle.Checked;
                        squad2.LeaderReactsToEvents = squad2LeaderReacts.Checked;
                        squad2.Invincible = squad2GodMode.Checked;

                        squad2.Died += (sender, args) =>
                        {
                            if (args.Ped.Exists() && args.Ped.Killer.Exists())
                            {
                                GTA.UI.Notification.Show(string.Format("Squad 2 member {0} killed by {1}", args.Index, args.Ped.Killer.Model.ToString()));
                            }
                        };
                    }
                }
                else if (e.Item == squad2Dismiss)
                {
                    if (squad2 != null)
                    {
                        squad2.Dismiss();
                        squad2 = null;
                        squad2FollowPlayer.Checked = false;
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

                        if (pedTypes.TryGetValue(squad3PedType.SelectedItem, out PedHash[] modelHashes))
                        {
                            Random rand = new Random();
                            selectedPed = modelHashes[rand.Next(1, modelHashes.Length)];
                        }

                        if (vehicleTypes.TryGetValue(squad3VehicleType.SelectedItem, out VehicleHash hash2)) selectedVehicle = hash2;
                        if (primaryWeaponTypes.TryGetValue(squad3MainWeapon.SelectedItem, out WeaponHash hash3)) selectedMainWeapon = hash3;
                        if (secondaryWeaponTypes.TryGetValue(squad3SecondWeapon.SelectedItem, out WeaponHash hash4)) selectedSecondWeapon = hash4;

                        squad3 = new Squad(BlipColor.Green, squad3NumPeds.SelectedItem, selectedPed, selectedVehicle, selectedMainWeapon, selectedSecondWeapon, !squad3HatesPlayer.Checked);
                        if (squad1 != null) squad3.SetRelationshipWithGroup(squad1.GetRelationshipGroup(), squad3HatesSquad1.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad2 != null) squad3.SetRelationshipWithGroup(squad2.GetRelationshipGroup(), squad3HatesSquad2.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad4 != null) squad3.SetRelationshipWithGroup(squad4.GetRelationshipGroup(), squad3HatesSquad4.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad5 != null) squad3.SetRelationshipWithGroup(squad5.GetRelationshipGroup(), squad3HatesSquad5.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad6 != null) squad3.SetRelationshipWithGroup(squad6.GetRelationshipGroup(), squad3HatesSquad6.Checked ? Relationship.Hate : Relationship.Companion, true);
                        foreach (KeyValuePair<string, Relationship> kvp in squad3CachedRelations)
                        {
                            int hash = Game.GenerateHash(kvp.Key);
                            squad3.SetRelationshipWithGroup(new RelationshipGroup(hash), kvp.Value);
                        }
                        squad3.Spawn();
                        squad3.CanExitVehicle = squad3CanExitVehicle.Checked;
                        squad3.LeaderReactsToEvents = squad3LeaderReacts.Checked;
                        squad3.Invincible = squad3GodMode.Checked;

                        squad3.Died += (sender, args) =>
                        {
                            if (args.Ped.Exists() && args.Ped.Killer.Exists())
                            {
                                GTA.UI.Notification.Show(string.Format("Squad 3 member {0} killed by {1}", args.Index, args.Ped.Killer.Model.ToString()));
                            }
                        };
                    }
                }
                else if (e.Item == squad3Dismiss)
                {
                    if (squad3 != null)
                    {
                        squad3.Dismiss();
                        squad3 = null;
                        squad3FollowPlayer.Checked = false;
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

                        if (pedTypes.TryGetValue(squad4PedType.SelectedItem, out PedHash[] modelHashes))
                        {
                            Random rand = new Random();
                            selectedPed = modelHashes[rand.Next(1, modelHashes.Length)];
                        }

                        if (vehicleTypes.TryGetValue(squad4VehicleType.SelectedItem, out VehicleHash hash2)) selectedVehicle = hash2;
                        if (primaryWeaponTypes.TryGetValue(squad4MainWeapon.SelectedItem, out WeaponHash hash3)) selectedMainWeapon = hash3;
                        if (secondaryWeaponTypes.TryGetValue(squad4SecondWeapon.SelectedItem, out WeaponHash hash4)) selectedSecondWeapon = hash4;

                        squad4 = new Squad(BlipColor.Red, squad4NumPeds.SelectedItem, selectedPed, selectedVehicle, selectedMainWeapon, selectedSecondWeapon, !squad4HatesPlayer.Checked);
                        if (squad1 != null) squad4.SetRelationshipWithGroup(squad1.GetRelationshipGroup(), squad4HatesSquad1.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad2 != null) squad4.SetRelationshipWithGroup(squad2.GetRelationshipGroup(), squad4HatesSquad2.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad3 != null) squad4.SetRelationshipWithGroup(squad3.GetRelationshipGroup(), squad4HatesSquad3.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad5 != null) squad4.SetRelationshipWithGroup(squad5.GetRelationshipGroup(), squad4HatesSquad5.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad6 != null) squad4.SetRelationshipWithGroup(squad6.GetRelationshipGroup(), squad4HatesSquad6.Checked ? Relationship.Hate : Relationship.Companion, true);
                        foreach (KeyValuePair<string, Relationship> kvp in squad4CachedRelations)
                        {
                            int hash = Game.GenerateHash(kvp.Key);
                            squad4.SetRelationshipWithGroup(new RelationshipGroup(hash), kvp.Value);
                        }
                        squad4.Spawn();
                        squad4.CanExitVehicle = squad4CanExitVehicle.Checked;
                        squad4.LeaderReactsToEvents = squad4LeaderReacts.Checked;
                        squad4.Invincible = squad4GodMode.Checked;

                        squad4.Died += (sender, args) =>
                        {
                            if (args.Ped.Exists() && args.Ped.Killer.Exists())
                            {
                                GTA.UI.Notification.Show(string.Format("Squad 4 member {0} killed by {1}", args.Index, args.Ped.Killer.Model.ToString()));
                            }
                        };
                    }
                }
                else if (e.Item == squad4Dismiss)
                {
                    if (squad4 != null)
                    {
                        squad4.Dismiss();
                        squad4 = null;
                        squad4FollowPlayer.Checked = false;
                    }
                }
            };

            squad5Menu.ItemActivated += (s, e) =>
            {
                if (e.Item == squad5Call)
                {
                    if (squad5 == null)
                    {
                        PedHash selectedPed = PedHash.MerryWeatherCutscene;
                        VehicleHash selectedVehicle = VehicleHash.Buzzard;
                        WeaponHash selectedMainWeapon = WeaponHash.MicroSMG;
                        WeaponHash selectedSecondWeapon = WeaponHash.Pistol;

                        if (pedTypes.TryGetValue(squad5PedType.SelectedItem, out PedHash[] modelHashes))
                        {
                            Random rand = new Random();
                            selectedPed = modelHashes[rand.Next(1, modelHashes.Length)];
                        }

                        if (vehicleTypes.TryGetValue(squad5VehicleType.SelectedItem, out VehicleHash hash2)) selectedVehicle = hash2;
                        if (primaryWeaponTypes.TryGetValue(squad5MainWeapon.SelectedItem, out WeaponHash hash3)) selectedMainWeapon = hash3;
                        if (secondaryWeaponTypes.TryGetValue(squad5SecondWeapon.SelectedItem, out WeaponHash hash4)) selectedSecondWeapon = hash4;

                        squad5 = new Squad(BlipColor.Orange, squad5NumPeds.SelectedItem, selectedPed, selectedVehicle, selectedMainWeapon, selectedSecondWeapon, !squad5HatesPlayer.Checked);
                        if (squad1 != null) squad5.SetRelationshipWithGroup(squad1.GetRelationshipGroup(), squad5HatesSquad1.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad2 != null) squad5.SetRelationshipWithGroup(squad2.GetRelationshipGroup(), squad5HatesSquad2.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad3 != null) squad5.SetRelationshipWithGroup(squad3.GetRelationshipGroup(), squad5HatesSquad3.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad4 != null) squad5.SetRelationshipWithGroup(squad4.GetRelationshipGroup(), squad5HatesSquad4.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad6 != null) squad5.SetRelationshipWithGroup(squad6.GetRelationshipGroup(), squad5HatesSquad6.Checked ? Relationship.Hate : Relationship.Companion, true);
                        foreach (KeyValuePair<string, Relationship> kvp in squad5CachedRelations)
                        {
                            int hash = Game.GenerateHash(kvp.Key);
                            squad5.SetRelationshipWithGroup(new RelationshipGroup(hash), kvp.Value);
                        }
                        squad5.Spawn();
                        squad5.CanExitVehicle = squad5CanExitVehicle.Checked;
                        squad5.LeaderReactsToEvents = squad5LeaderReacts.Checked;
                        squad5.Invincible = squad5GodMode.Checked;

                        squad5.Died += (sender, args) =>
                        {
                            if (args.Ped.Exists() && args.Ped.Killer.Exists())
                            {
                                GTA.UI.Notification.Show(string.Format("Squad 5 member {0} killed by {1}", args.Index, args.Ped.Killer.Model.ToString()));
                            }
                        };
                    }
                }
                else if (e.Item == squad5Dismiss)
                {
                    if (squad5 != null)
                    {
                        squad5.Dismiss();
                        squad5 = null;
                        squad5FollowPlayer.Checked = false;
                    }
                }
            };

            squad6Menu.ItemActivated += (s, e) =>
            {
                if (e.Item == squad6Call)
                {
                    if (squad6 == null)
                    {
                        PedHash selectedPed = PedHash.MerryWeatherCutscene;
                        VehicleHash selectedVehicle = VehicleHash.Buzzard;
                        WeaponHash selectedMainWeapon = WeaponHash.MicroSMG;
                        WeaponHash selectedSecondWeapon = WeaponHash.Pistol;

                        if (pedTypes.TryGetValue(squad6PedType.SelectedItem, out PedHash[] modelHashes))
                        {
                            Random rand = new Random();
                            selectedPed = modelHashes[rand.Next(1, modelHashes.Length)];
                        }

                        if (vehicleTypes.TryGetValue(squad6VehicleType.SelectedItem, out VehicleHash hash2)) selectedVehicle = hash2;
                        if (primaryWeaponTypes.TryGetValue(squad6MainWeapon.SelectedItem, out WeaponHash hash3)) selectedMainWeapon = hash3;
                        if (secondaryWeaponTypes.TryGetValue(squad6SecondWeapon.SelectedItem, out WeaponHash hash4)) selectedSecondWeapon = hash4;

                        squad6 = new Squad(BlipColor.Purple, squad6NumPeds.SelectedItem, selectedPed, selectedVehicle, selectedMainWeapon, selectedSecondWeapon, !squad6HatesPlayer.Checked);
                        if (squad1 != null) squad6.SetRelationshipWithGroup(squad1.GetRelationshipGroup(), squad6HatesSquad1.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad2 != null) squad6.SetRelationshipWithGroup(squad2.GetRelationshipGroup(), squad6HatesSquad2.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad3 != null) squad6.SetRelationshipWithGroup(squad3.GetRelationshipGroup(), squad6HatesSquad3.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad4 != null) squad6.SetRelationshipWithGroup(squad4.GetRelationshipGroup(), squad6HatesSquad4.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad5 != null) squad6.SetRelationshipWithGroup(squad5.GetRelationshipGroup(), squad6HatesSquad5.Checked ? Relationship.Hate : Relationship.Companion, true);
                        foreach (KeyValuePair<string, Relationship> kvp in squad6CachedRelations)
                        {
                            int hash = Game.GenerateHash(kvp.Key);
                            squad6.SetRelationshipWithGroup(new RelationshipGroup(hash), kvp.Value);
                        }
                        squad6.Spawn();
                        squad6.CanExitVehicle = squad6CanExitVehicle.Checked;
                        squad6.LeaderReactsToEvents = squad6LeaderReacts.Checked;
                        squad6.Invincible = squad6GodMode.Checked;

                        squad6.Died += (sender, args) =>
                        {
                            if (args.Ped.Exists() && args.Ped.Killer.Exists())
                            {
                                GTA.UI.Notification.Show(string.Format("Squad 6 member {0} killed by {1}", args.Index, args.Ped.Killer.Model.ToString()));
                            }
                        };
                    }
                }
                else if (e.Item == squad6Dismiss)
                {
                    if (squad6 != null)
                    {
                        squad6.Dismiss();
                        squad6 = null;
                        squad6FollowPlayer.Checked = false;
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

            squad5CanExitVehicle.CheckboxChanged += (s, e) =>
            {
                if (squad5 != null && !squad5.Dead()) squad5.CanExitVehicle = squad5CanExitVehicle.Checked;
            };

            squad6CanExitVehicle.CheckboxChanged += (s, e) =>
            {
                if (squad6 != null && !squad6.Dead()) squad6.CanExitVehicle = squad6CanExitVehicle.Checked;
            };

            squad1LeaderReacts.CheckboxChanged += (s, e) =>
            {
                if (squad1 != null && !squad1.Dead()) squad1.LeaderReactsToEvents = squad1LeaderReacts.Checked;
            };

            squad2LeaderReacts.CheckboxChanged += (s, e) =>
            {
                if (squad2 != null && !squad2.Dead()) squad2.LeaderReactsToEvents = squad2LeaderReacts.Checked;
            };

            squad3LeaderReacts.CheckboxChanged += (s, e) =>
            {
                if (squad3 != null && !squad3.Dead()) squad3.LeaderReactsToEvents = squad3LeaderReacts.Checked;
            };

            squad4LeaderReacts.CheckboxChanged += (s, e) =>
            {
                if (squad4 != null && !squad4.Dead()) squad4.LeaderReactsToEvents = squad4LeaderReacts.Checked;
            };

            squad5LeaderReacts.CheckboxChanged += (s, e) =>
            {
                if (squad5 != null && !squad5.Dead()) squad5.LeaderReactsToEvents = squad5LeaderReacts.Checked;
            };

            squad6LeaderReacts.CheckboxChanged += (s, e) =>
            {
                if (squad6 != null && !squad6.Dead()) squad6.LeaderReactsToEvents = squad6LeaderReacts.Checked;
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

            squad5GodMode.CheckboxChanged += (s, e) =>
            {
                if (squad5 != null && !squad5.Dead()) squad5.Invincible = squad5GodMode.Checked;
            };

            squad6GodMode.CheckboxChanged += (s, e) =>
            {
                if (squad6 != null && !squad6.Dead()) squad6.Invincible = squad6GodMode.Checked;
            };

            squad1Tasks.ItemActivated += (s, e) =>
            {
                if (squad1 != null)
                {
                    if (e.Item == squad1EnterLeave)
                    {
                        squad1.EnterLeaveVehicle();
                    }
                    else if (e.Item == squad1DriveToWp)
                    {
                        if (World.WaypointPosition != Vector3.Zero)
                        {
                            // If vehicle is not helicopter, they will drive instead
                            squad1.FlyTo(World.WaypointPosition, 4, 0, 75.0f, 20.0f, DrivingStyle.Rushed);
                        }
                    }
                    else if (e.Item == squad1EscortMe)
                    {
                        squad1.EscortVehicle(Game.Player.Character.CurrentVehicle, 50.0f, DrivingStyle.Rushed);
                    }
                    else if (e.Item == squad1RunToMe)
                    {
                        Vector3 playerPos = Game.Player.Character.Position;
                        squad1.RunTo(new Vector3(playerPos.X + 5, playerPos.Y, playerPos.Z));
                    }
                }
            };

            squad2Tasks.ItemActivated += (s, e) =>
            {
                if (squad2 != null)
                {
                    if (e.Item == squad2EnterLeave)
                    {
                        squad2.EnterLeaveVehicle();
                    }
                    else if (e.Item == squad2DriveToWp)
                    {
                        if (World.WaypointPosition != Vector3.Zero)
                        {
                            // If vehicle is not helicopter, they will drive instead
                            squad2.FlyTo(World.WaypointPosition, 4, 0, 75.0f, 20.0f, DrivingStyle.Rushed);
                        }
                    }
                    else if (e.Item == squad2EscortMe)
                    {
                        squad2.EscortVehicle(Game.Player.Character.CurrentVehicle, 50.0f, DrivingStyle.Rushed);
                    }
                    else if (e.Item == squad2RunToMe)
                    {
                        Vector3 playerPos = Game.Player.Character.Position;
                        squad2.RunTo(new Vector3(playerPos.X + 5, playerPos.Y, playerPos.Z));
                    }
                }
            };

            squad3Tasks.ItemActivated += (s, e) =>
            {
                if (squad3 != null)
                {
                    if (e.Item == squad3EnterLeave)
                    {
                        squad3.EnterLeaveVehicle();
                    }
                    else if (e.Item == squad3DriveToWp)
                    {
                        if (World.WaypointPosition != Vector3.Zero)
                        {
                            // If vehicle is not helicopter, they will drive instead
                            squad3.FlyTo(World.WaypointPosition, 4, 0, 75.0f, 20.0f, DrivingStyle.Rushed);
                        }
                    }
                    else if (e.Item == squad3EscortMe)
                    {
                        squad3.EscortVehicle(Game.Player.Character.CurrentVehicle, 50.0f, DrivingStyle.Rushed);
                    }
                    else if (e.Item == squad3RunToMe)
                    {
                        Vector3 playerPos = Game.Player.Character.Position;
                        squad3.RunTo(new Vector3(playerPos.X + 5, playerPos.Y, playerPos.Z));
                    }
                }
            };

            squad4Tasks.ItemActivated += (s, e) =>
            {
                if (squad4 != null)
                {
                    if (e.Item == squad4EnterLeave)
                    {
                        squad4.EnterLeaveVehicle();
                    }
                    else if (e.Item == squad4DriveToWp)
                    {
                        if (World.WaypointPosition != Vector3.Zero)
                        {
                            // If vehicle is not helicopter, they will drive instead
                            squad4.FlyTo(World.WaypointPosition, 4, 0, 75.0f, 20.0f, DrivingStyle.Rushed);
                        }
                    }
                    else if (e.Item == squad4EscortMe)
                    {
                        squad4.EscortVehicle(Game.Player.Character.CurrentVehicle, 50.0f, DrivingStyle.Rushed);
                    }
                    else if (e.Item == squad4RunToMe)
                    {
                        Vector3 playerPos = Game.Player.Character.Position;
                        squad4.RunTo(new Vector3(playerPos.X + 5, playerPos.Y, playerPos.Z));
                    }
                }
            };

            squad5Tasks.ItemActivated += (s, e) =>
            {
                if (squad5 != null)
                {
                    if (e.Item == squad5EnterLeave)
                    {
                        squad5.EnterLeaveVehicle();
                    }
                    else if (e.Item == squad5DriveToWp)
                    {
                        if (World.WaypointPosition != Vector3.Zero)
                        {
                            // If vehicle is not helicopter, they will drive instead
                            squad5.FlyTo(World.WaypointPosition, 4, 0, 75.0f, 20.0f, DrivingStyle.Rushed);
                        }
                    }
                    else if (e.Item == squad5EscortMe)
                    {
                        squad5.EscortVehicle(Game.Player.Character.CurrentVehicle, 50.0f, DrivingStyle.Rushed);
                    }
                    else if (e.Item == squad5RunToMe)
                    {
                        Vector3 playerPos = Game.Player.Character.Position;
                        squad5.RunTo(new Vector3(playerPos.X + 5, playerPos.Y, playerPos.Z));
                    }
                }
            };

            squad6Tasks.ItemActivated += (s, e) =>
            {
                if (squad6 != null)
                {
                    if (e.Item == squad6EnterLeave)
                    {
                        squad6.EnterLeaveVehicle();
                    }
                    else if (e.Item == squad6DriveToWp)
                    {
                        if (World.WaypointPosition != Vector3.Zero)
                        {
                            // If vehicle is not helicopter, they will drive instead
                            squad6.FlyTo(World.WaypointPosition, 4, 0, 75.0f, 20.0f, DrivingStyle.Rushed);
                        }
                    }
                    else if (e.Item == squad6EscortMe)
                    {
                        squad6.EscortVehicle(Game.Player.Character.CurrentVehicle, 50.0f, DrivingStyle.Rushed);
                    }
                    else if (e.Item == squad6RunToMe)
                    {
                        Vector3 playerPos = Game.Player.Character.Position;
                        squad6.RunTo(new Vector3(playerPos.X + 5, playerPos.Y, playerPos.Z));
                    }
                }
            };

            squad1FollowPlayer.CheckboxChanged += (s, e) =>
            {
                if (squad1 != null) squad1.FollowPlayer(squad1FollowPlayer.Checked);
            };

            squad2FollowPlayer.CheckboxChanged += (s, e) =>
            {
                if (squad2 != null) squad2.FollowPlayer(squad2FollowPlayer.Checked);
            };

            squad3FollowPlayer.CheckboxChanged += (s, e) =>
            {
                if (squad3 != null) squad3.FollowPlayer(squad3FollowPlayer.Checked);
            };

            squad4FollowPlayer.CheckboxChanged += (s, e) =>
            {
                if (squad4 != null) squad4.FollowPlayer(squad4FollowPlayer.Checked);
            };

            squad5FollowPlayer.CheckboxChanged += (s, e) =>
            {
                if (squad5 != null) squad5.FollowPlayer(squad5FollowPlayer.Checked);
            };

            squad6FollowPlayer.CheckboxChanged += (s, e) =>
            {
                if (squad6 != null) squad6.FollowPlayer(squad6FollowPlayer.Checked);
            };

            squad1Relations.ItemActivated += (s, e) =>
            {
                if (e.Item == squad1RelationsHate)
                {
                    string input = Game.GetUserInput();
                    if (!string.IsNullOrEmpty(input))
                    {
                        if (squad1CachedRelations.Remove(input))
                        {
                            GTA.UI.Screen.ShowSubtitle($"Squad 1 is now neutral with {input}.");
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
                        if (squad2CachedRelations.Remove(input))
                        {
                            GTA.UI.Screen.ShowSubtitle($"Squad 2 is now neutral with {input}.");
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
                        if (squad3CachedRelations.Remove(input))
                        {
                            GTA.UI.Screen.ShowSubtitle($"Squad 3 is now neutral with {input}.");
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
                        if (squad4CachedRelations.Remove(input))
                        {
                            GTA.UI.Screen.ShowSubtitle($"Squad 4 is now neutral with {input}.");
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

            squad5Relations.ItemActivated += (s, e) =>
            {
                if (e.Item == squad5RelationsHate)
                {
                    string input = Game.GetUserInput();
                    if (!string.IsNullOrEmpty(input))
                    {
                        if (squad5CachedRelations.Remove(input))
                        {
                            GTA.UI.Screen.ShowSubtitle($"Squad 5 is now neutral with {input}.");
                            if (squad5 != null)
                            {
                                int hash = Game.GenerateHash(input);
                                squad5.ClearRelationshipWithGroup(new RelationshipGroup(hash), Relationship.Hate);
                            }
                        }
                        else
                        {
                            GTA.UI.Screen.ShowSubtitle($"Squad 5 now hates {input}.");
                            squad5CachedRelations.Add(input, Relationship.Hate);
                            if (squad5 != null)
                            {
                                int hash = Game.GenerateHash(input);
                                squad5.SetRelationshipWithGroup(new RelationshipGroup(hash), Relationship.Hate);
                            }
                        }
                    }
                }
            };

            squad6Relations.ItemActivated += (s, e) =>
            {
                if (e.Item == squad6RelationsHate)
                {
                    string input = Game.GetUserInput();
                    if (!string.IsNullOrEmpty(input))
                    {
                        if (squad6CachedRelations.Remove(input))
                        {
                            GTA.UI.Screen.ShowSubtitle($"Squad 6 is now neutral with {input}.");
                            if (squad6 != null)
                            {
                                int hash = Game.GenerateHash(input);
                                squad6.ClearRelationshipWithGroup(new RelationshipGroup(hash), Relationship.Hate);
                            }
                        }
                        else
                        {
                            GTA.UI.Screen.ShowSubtitle($"Squad 6 now hates {input}.");
                            squad6CachedRelations.Add(input, Relationship.Hate);
                            if (squad6 != null)
                            {
                                int hash = Game.GenerateHash(input);
                                squad6.SetRelationshipWithGroup(new RelationshipGroup(hash), Relationship.Hate);
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

            squad5HatesPlayer.CheckboxChanged += (s, e) =>
            {
                if (squad5 != null) squad5.SetRelationshipWithGroup(Game.Player.Character.RelationshipGroup, squad5HatesPlayer.Checked ? Relationship.Hate : Relationship.Companion, true);
            };

            squad6HatesPlayer.CheckboxChanged += (s, e) =>
            {
                if (squad6 != null) squad6.SetRelationshipWithGroup(Game.Player.Character.RelationshipGroup, squad6HatesPlayer.Checked ? Relationship.Hate : Relationship.Companion, true);
            };

            // Hate checkboxes change bidirectionally!!


            // Squad 1

            squad1HatesSquad2.CheckboxChanged += (s, e) =>
            {
                if (squad1 != null && squad2 != null) squad1.SetRelationshipWithGroup(squad2.GetRelationshipGroup(), squad1HatesSquad2.Checked ? Relationship.Hate : Relationship.Companion, true);
                squad2HatesSquad1.Checked = squad1HatesSquad2.Checked;
            };

            squad1HatesSquad3.CheckboxChanged += (s, e) =>
            {
                if (squad1 != null && squad3 != null) squad1.SetRelationshipWithGroup(squad3.GetRelationshipGroup(), squad1HatesSquad3.Checked ? Relationship.Hate : Relationship.Companion, true);
                squad3HatesSquad1.Checked = squad1HatesSquad3.Checked;
            };

            squad1HatesSquad4.CheckboxChanged += (s, e) =>
            {
                if (squad1 != null && squad4 != null) squad1.SetRelationshipWithGroup(squad4.GetRelationshipGroup(), squad1HatesSquad4.Checked ? Relationship.Hate : Relationship.Companion, true);
                squad4HatesSquad1.Checked = squad1HatesSquad4.Checked;
            };

            squad1HatesSquad5.CheckboxChanged += (s, e) =>
            {
                if (squad1 != null && squad5 != null) squad1.SetRelationshipWithGroup(squad5.GetRelationshipGroup(), squad1HatesSquad5.Checked ? Relationship.Hate : Relationship.Companion, true);
                squad5HatesSquad1.Checked = squad1HatesSquad5.Checked;
            };

            squad1HatesSquad6.CheckboxChanged += (s, e) =>
            {
                if (squad1 != null && squad6 != null) squad1.SetRelationshipWithGroup(squad6.GetRelationshipGroup(), squad1HatesSquad6.Checked ? Relationship.Hate : Relationship.Companion, true);
                squad6HatesSquad1.Checked = squad1HatesSquad6.Checked;
            };

            // Squad 2

            squad2HatesSquad1.CheckboxChanged += (s, e) =>
            {
                if (squad2 != null && squad1 != null) squad2.SetRelationshipWithGroup(squad1.GetRelationshipGroup(), squad2HatesSquad1.Checked ? Relationship.Hate : Relationship.Companion, true);
                squad1HatesSquad2.Checked = squad2HatesSquad1.Checked;
            };

            squad2HatesSquad3.CheckboxChanged += (s, e) =>
            {
                if (squad2 != null && squad3 != null) squad2.SetRelationshipWithGroup(squad3.GetRelationshipGroup(), squad2HatesSquad3.Checked ? Relationship.Hate : Relationship.Companion, true);
                squad3HatesSquad2.Checked = squad2HatesSquad3.Checked;
            };

            squad2HatesSquad4.CheckboxChanged += (s, e) =>
            {
                if (squad2 != null && squad4 != null) squad2.SetRelationshipWithGroup(squad4.GetRelationshipGroup(), squad2HatesSquad4.Checked ? Relationship.Hate : Relationship.Companion, true);
                squad4HatesSquad2.Checked = squad2HatesSquad4.Checked;
            };

            squad2HatesSquad5.CheckboxChanged += (s, e) =>
            {
                if (squad2 != null && squad5 != null) squad2.SetRelationshipWithGroup(squad5.GetRelationshipGroup(), squad2HatesSquad5.Checked ? Relationship.Hate : Relationship.Companion, true);
                squad5HatesSquad2.Checked = squad2HatesSquad5.Checked;
            };

            squad2HatesSquad6.CheckboxChanged += (s, e) =>
            {
                if (squad2 != null && squad6 != null) squad2.SetRelationshipWithGroup(squad6.GetRelationshipGroup(), squad2HatesSquad6.Checked ? Relationship.Hate : Relationship.Companion, true);
                squad6HatesSquad2.Checked = squad2HatesSquad6.Checked;
            };

            // Squad 3

            squad3HatesSquad1.CheckboxChanged += (s, e) =>
            {
                if (squad3 != null && squad1 != null) squad3.SetRelationshipWithGroup(squad1.GetRelationshipGroup(), squad3HatesSquad1.Checked ? Relationship.Hate : Relationship.Companion, true);
                squad1HatesSquad3.Checked = squad3HatesSquad1.Checked;
            };

            squad3HatesSquad2.CheckboxChanged += (s, e) =>
            {
                if (squad3 != null && squad2 != null) squad3.SetRelationshipWithGroup(squad2.GetRelationshipGroup(), squad3HatesSquad2.Checked ? Relationship.Hate : Relationship.Companion, true);
                squad2HatesSquad3.Checked = squad3HatesSquad2.Checked;
            };

            squad3HatesSquad4.CheckboxChanged += (s, e) =>
            {
                if (squad3 != null && squad4 != null) squad3.SetRelationshipWithGroup(squad4.GetRelationshipGroup(), squad3HatesSquad4.Checked ? Relationship.Hate : Relationship.Companion, true);
                squad4HatesSquad3.Checked = squad3HatesSquad4.Checked;
            };

            squad3HatesSquad5.CheckboxChanged += (s, e) =>
            {
                if (squad3 != null && squad5 != null) squad3.SetRelationshipWithGroup(squad5.GetRelationshipGroup(), squad3HatesSquad5.Checked ? Relationship.Hate : Relationship.Companion, true);
                squad5HatesSquad3.Checked = squad3HatesSquad5.Checked;
            };

            squad3HatesSquad6.CheckboxChanged += (s, e) =>
            {
                if (squad3 != null && squad6 != null) squad3.SetRelationshipWithGroup(squad6.GetRelationshipGroup(), squad3HatesSquad6.Checked ? Relationship.Hate : Relationship.Companion, true);
                squad6HatesSquad3.Checked = squad3HatesSquad6.Checked;
            };

            // Squad 4

            squad4HatesSquad1.CheckboxChanged += (s, e) =>
            {
                if (squad4 != null && squad1 != null) squad4.SetRelationshipWithGroup(squad1.GetRelationshipGroup(), squad4HatesSquad1.Checked ? Relationship.Hate : Relationship.Companion, true);
                squad1HatesSquad4.Checked = squad4HatesSquad1.Checked;
            };

            squad4HatesSquad2.CheckboxChanged += (s, e) =>
            {
                if (squad4 != null && squad2 != null) squad4.SetRelationshipWithGroup(squad2.GetRelationshipGroup(), squad4HatesSquad2.Checked ? Relationship.Hate : Relationship.Companion, true);
                squad2HatesSquad4.Checked = squad4HatesSquad2.Checked;
            };

            squad4HatesSquad3.CheckboxChanged += (s, e) =>
            {
                if (squad4 != null && squad3 != null) squad4.SetRelationshipWithGroup(squad3.GetRelationshipGroup(), squad4HatesSquad3.Checked ? Relationship.Hate : Relationship.Companion, true);
                squad3HatesSquad4.Checked = squad4HatesSquad3.Checked;
            };

            squad4HatesSquad5.CheckboxChanged += (s, e) =>
            {
                if (squad4 != null && squad5 != null) squad4.SetRelationshipWithGroup(squad5.GetRelationshipGroup(), squad4HatesSquad5.Checked ? Relationship.Hate : Relationship.Companion, true);
                squad5HatesSquad4.Checked = squad4HatesSquad5.Checked;
            };

            squad4HatesSquad6.CheckboxChanged += (s, e) =>
            {
                if (squad4 != null && squad6 != null) squad4.SetRelationshipWithGroup(squad6.GetRelationshipGroup(), squad4HatesSquad6.Checked ? Relationship.Hate : Relationship.Companion, true);
                squad6HatesSquad4.Checked = squad4HatesSquad6.Checked;
            };

            // Squad 5

            squad5HatesSquad1.CheckboxChanged += (s, e) =>
            {
                if (squad5 != null && squad1 != null) squad5.SetRelationshipWithGroup(squad1.GetRelationshipGroup(), squad5HatesSquad1.Checked ? Relationship.Hate : Relationship.Companion, true);
                squad1HatesSquad5.Checked = squad5HatesSquad1.Checked;
            };

            squad5HatesSquad2.CheckboxChanged += (s, e) =>
            {
                if (squad5 != null && squad2 != null) squad5.SetRelationshipWithGroup(squad2.GetRelationshipGroup(), squad5HatesSquad2.Checked ? Relationship.Hate : Relationship.Companion, true);
                squad2HatesSquad5.Checked = squad5HatesSquad2.Checked;
            };

            squad5HatesSquad3.CheckboxChanged += (s, e) =>
            {
                if (squad5 != null && squad3 != null) squad5.SetRelationshipWithGroup(squad3.GetRelationshipGroup(), squad5HatesSquad3.Checked ? Relationship.Hate : Relationship.Companion, true);
                squad3HatesSquad5.Checked = squad5HatesSquad3.Checked;
            };

            squad5HatesSquad4.CheckboxChanged += (s, e) =>
            {
                if (squad5 != null && squad4 != null) squad5.SetRelationshipWithGroup(squad4.GetRelationshipGroup(), squad5HatesSquad4.Checked ? Relationship.Hate : Relationship.Companion, true);
                squad4HatesSquad5.Checked = squad5HatesSquad4.Checked;
            };

            squad5HatesSquad6.CheckboxChanged += (s, e) =>
            {
                if (squad5 != null && squad6 != null) squad5.SetRelationshipWithGroup(squad6.GetRelationshipGroup(), squad5HatesSquad6.Checked ? Relationship.Hate : Relationship.Companion, true);
                squad6HatesSquad5.Checked = squad5HatesSquad6.Checked;
            };

            pool.Add(mainMenu);

            pool.Add(squad1Menu);
            pool.Add(squad2Menu);
            pool.Add(squad3Menu);
            pool.Add(squad4Menu);
            pool.Add(squad5Menu);
            pool.Add(squad6Menu);

            pool.Add(squad1Settings);
            pool.Add(squad2Settings);
            pool.Add(squad3Settings);
            pool.Add(squad4Settings);
            pool.Add(squad5Settings);
            pool.Add(squad6Settings);

            pool.Add(squad1Tasks);
            pool.Add(squad2Tasks);
            pool.Add(squad3Tasks);
            pool.Add(squad4Tasks);
            pool.Add(squad5Tasks);
            pool.Add(squad6Tasks);

            pool.Add(squad1Relations);
            pool.Add(squad2Relations);
            pool.Add(squad3Relations);
            pool.Add(squad4Relations);
            pool.Add(squad5Relations);
            pool.Add(squad6Relations);

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

        private SizeF GetScreenResolutionMaintainRatio()
        {
            int screenW = GTA.UI.Screen.Resolution.Width;
            int screenH = GTA.UI.Screen.Resolution.Height;

            float ratio = (float)screenW / screenH;

            float width = 1080f * ratio;

            return new SizeF(width, 1080f);
        }

        private Point GetSafezoneBounds()
        {
            float t = GTA.Native.Function.Call<float>(GTA.Native.Hash.GET_SAFE_ZONE_SIZE);
            double g = Math.Round(Convert.ToDouble(t), 2);
            g = (g * 100) - 90;
            g = 10 - g;

            int screenW = GTA.UI.Screen.Resolution.Width;
            int screenH = GTA.UI.Screen.Resolution.Height;

            float ratio = (float)screenW / screenH;

            float wmp = ratio * 5.4f;

            return new Point((int)Math.Round(g * wmp), (int)Math.Round(g * 5.4f));
        }

        private void onTick(object sender, EventArgs e)
        {
            pool.Process();

            /* SizeF screenResolutionMaintainRatio = GetScreenResolutionMaintainRatio();
            Point safezoneBounds = GetSafezoneBounds();

            int num = 0;
            if (squad1 != null && squad1.Count > 0)
            {
                num += 35;

                LemonUI.Elements.ScaledTexture sprite1 = new LemonUI.Elements.ScaledTexture(new Point(Convert.ToInt32(screenResolutionMaintainRatio.Width) - safezoneBounds.X - num, Convert.ToInt32(screenResolutionMaintainRatio.Height) - safezoneBounds.Y - 364), new Size(34, 250), "timerbars", "all_black_bg");
                sprite1.Heading = 270f;
                sprite1.Color = Color.FromArgb(23, 100, 141);
                sprite1.Draw();
            } */

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
                        if (squad2 != null) squad1.SetRelationshipWithGroup(squad2.GetRelationshipGroup(), squad1HatesSquad2.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad3 != null) squad1.SetRelationshipWithGroup(squad3.GetRelationshipGroup(), squad1HatesSquad3.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad4 != null) squad1.SetRelationshipWithGroup(squad4.GetRelationshipGroup(), squad1HatesSquad4.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad5 != null) squad1.SetRelationshipWithGroup(squad5.GetRelationshipGroup(), squad1HatesSquad5.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad6 != null) squad1.SetRelationshipWithGroup(squad6.GetRelationshipGroup(), squad1HatesSquad6.Checked ? Relationship.Hate : Relationship.Companion, true);
                        foreach (KeyValuePair<string, Relationship> kvp in squad1CachedRelations)
                        {
                            int hash = Game.GenerateHash(kvp.Key);
                            squad1.SetRelationshipWithGroup(new RelationshipGroup(hash), kvp.Value);
                        }
                        squad1.Spawn();
                        squad1.CanExitVehicle = squad1CanExitVehicle.Checked;
                        squad1.LeaderReactsToEvents = squad1LeaderReacts.Checked;
                        squad1.Invincible = squad1GodMode.Checked;
                    }
                    else
                    {
                        squad1.RemovePedBlips();
                        squad1.RemoveVehicleBlip();
                        squad1 = null;
                    }
                }
                else
                {
                    foreach (Ped ped in squad1.Peds)
                    {
                        if (ped.Exists())
                        {
                            squad1.SetPedBlipSprite(ped, ped.IsInCombat ? BlipSprite.ShootingRange : BlipSprite.VIP);
                        }
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
                        if (squad1 != null) squad2.SetRelationshipWithGroup(squad1.GetRelationshipGroup(), squad2HatesSquad1.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad3 != null) squad2.SetRelationshipWithGroup(squad3.GetRelationshipGroup(), squad2HatesSquad3.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad4 != null) squad2.SetRelationshipWithGroup(squad4.GetRelationshipGroup(), squad2HatesSquad4.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad5 != null) squad2.SetRelationshipWithGroup(squad5.GetRelationshipGroup(), squad2HatesSquad5.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad6 != null) squad2.SetRelationshipWithGroup(squad6.GetRelationshipGroup(), squad2HatesSquad6.Checked ? Relationship.Hate : Relationship.Companion, true);
                        foreach (KeyValuePair<string, Relationship> kvp in squad2CachedRelations)
                        {
                            int hash = Game.GenerateHash(kvp.Key);
                            squad2.SetRelationshipWithGroup(new RelationshipGroup(hash), kvp.Value);
                        }
                        squad2.Spawn();
                        squad2.CanExitVehicle = squad2CanExitVehicle.Checked;
                        squad2.LeaderReactsToEvents = squad2LeaderReacts.Checked;
                        squad2.Invincible = squad2GodMode.Checked;
                    }
                    else
                    {
                        squad2.RemovePedBlips();
                        squad2.RemoveVehicleBlip();
                        squad2 = null;
                    }
                }
				else
				{
					foreach (Ped ped in squad2.Peds)
                    {
                        if (ped.Exists())
                        {
                            squad2.SetPedBlipSprite(ped, ped.IsInCombat ? BlipSprite.ShootingRange : BlipSprite.VIP);
                        }
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
                        if (squad1 != null) squad3.SetRelationshipWithGroup(squad1.GetRelationshipGroup(), squad3HatesSquad1.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad2 != null) squad3.SetRelationshipWithGroup(squad2.GetRelationshipGroup(), squad3HatesSquad2.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad4 != null) squad3.SetRelationshipWithGroup(squad4.GetRelationshipGroup(), squad3HatesSquad4.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad5 != null) squad3.SetRelationshipWithGroup(squad5.GetRelationshipGroup(), squad3HatesSquad5.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad6 != null) squad3.SetRelationshipWithGroup(squad6.GetRelationshipGroup(), squad3HatesSquad6.Checked ? Relationship.Hate : Relationship.Companion, true);
                        foreach (KeyValuePair<string, Relationship> kvp in squad3CachedRelations)
                        {
                            int hash = Game.GenerateHash(kvp.Key);
                            squad3.SetRelationshipWithGroup(new RelationshipGroup(hash), kvp.Value);
                        }
                        squad3.Spawn();
                        squad3.CanExitVehicle = squad3CanExitVehicle.Checked;
                        squad3.LeaderReactsToEvents = squad3LeaderReacts.Checked;
                        squad3.Invincible = squad3GodMode.Checked;
                    }
                    else
                    {
                        squad3.RemovePedBlips();
                        squad3.RemoveVehicleBlip();
                        squad3 = null;
                    }
                }
				else
				{
					foreach (Ped ped in squad3.Peds)
                    {
                        if (ped.Exists())
                        {
                            squad3.SetPedBlipSprite(ped, ped.IsInCombat ? BlipSprite.ShootingRange : BlipSprite.VIP);
                        }
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
                        if (squad1 != null) squad4.SetRelationshipWithGroup(squad1.GetRelationshipGroup(), squad4HatesSquad1.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad2 != null) squad4.SetRelationshipWithGroup(squad2.GetRelationshipGroup(), squad4HatesSquad2.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad3 != null) squad4.SetRelationshipWithGroup(squad3.GetRelationshipGroup(), squad4HatesSquad3.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad5 != null) squad4.SetRelationshipWithGroup(squad5.GetRelationshipGroup(), squad4HatesSquad5.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad6 != null) squad4.SetRelationshipWithGroup(squad6.GetRelationshipGroup(), squad4HatesSquad6.Checked ? Relationship.Hate : Relationship.Companion, true);
                        foreach (KeyValuePair<string, Relationship> kvp in squad4CachedRelations)
                        {
                            int hash = Game.GenerateHash(kvp.Key);
                            squad4.SetRelationshipWithGroup(new RelationshipGroup(hash), kvp.Value);
                        }
                        squad4.Spawn();
                        squad4.CanExitVehicle = squad4CanExitVehicle.Checked;
                        squad4.LeaderReactsToEvents = squad4LeaderReacts.Checked;
                        squad4.Invincible = squad4GodMode.Checked;
                    }
                    else
                    {
                        squad4.RemovePedBlips();
                        squad4.RemoveVehicleBlip();
                        squad4 = null;
                    }
                }
				else
				{
					foreach (Ped ped in squad4.Peds)
                    {
                        if (ped.Exists())
                        {
                            squad4.SetPedBlipSprite(ped, ped.IsInCombat ? BlipSprite.ShootingRange : BlipSprite.VIP);
                        }
                    }
				}
            }

            if (squad5 != null)
            {
                // Check if all peds are in vehicle, if so then add vehicle blip
                if (squad5.Vehicle != null && squad5.Vehicle.Exists())
                {
                    if (squad5.InVehicle)
                    {
                        squad5.RemovePedBlips();
                        squad5.AttachVehicleBlip();
                    }
                    else
                    {
                        squad5.AttachPedBlips();
                        squad5.RemoveVehicleBlip();
                    }
                }

                if (squad5.Dead())
                {
                    if (squad5AutoRespawn.Checked)
                    {
                        if (squad1 != null) squad5.SetRelationshipWithGroup(squad1.GetRelationshipGroup(), squad5HatesSquad1.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad2 != null) squad5.SetRelationshipWithGroup(squad2.GetRelationshipGroup(), squad5HatesSquad2.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad3 != null) squad5.SetRelationshipWithGroup(squad3.GetRelationshipGroup(), squad5HatesSquad3.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad4 != null) squad5.SetRelationshipWithGroup(squad4.GetRelationshipGroup(), squad5HatesSquad4.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad6 != null) squad5.SetRelationshipWithGroup(squad6.GetRelationshipGroup(), squad5HatesSquad6.Checked ? Relationship.Hate : Relationship.Companion, true);
                        foreach (KeyValuePair<string, Relationship> kvp in squad5CachedRelations)
                        {
                            int hash = Game.GenerateHash(kvp.Key);
                            squad5.SetRelationshipWithGroup(new RelationshipGroup(hash), kvp.Value);
                        }
                        squad5.Spawn();
                        squad5.CanExitVehicle = squad5CanExitVehicle.Checked;
                        squad5.LeaderReactsToEvents = squad5LeaderReacts.Checked;
                        squad5.Invincible = squad5GodMode.Checked;
                    }
                    else
                    {
                        squad5.RemovePedBlips();
                        squad5.RemoveVehicleBlip();
                        squad5 = null;
                    }
                }
                else
                {
                    foreach (Ped ped in squad5.Peds)
                    {
                        if (ped.Exists())
                        {
                            squad5.SetPedBlipSprite(ped, ped.IsInCombat ? BlipSprite.ShootingRange : BlipSprite.VIP);
                        }
                    }
                }
            }

            if (squad6 != null)
            {
                // Check if all peds are in vehicle, if so then add vehicle blip
                if (squad6.Vehicle != null && squad6.Vehicle.Exists())
                {
                    if (squad6.InVehicle)
                    {
                        squad6.RemovePedBlips();
                        squad6.AttachVehicleBlip();
                    }
                    else
                    {
                        squad6.AttachPedBlips();
                        squad6.RemoveVehicleBlip();
                    }
                }

                if (squad6.Dead())
                {
                    if (squad6AutoRespawn.Checked)
                    {
                        if (squad1 != null) squad6.SetRelationshipWithGroup(squad1.GetRelationshipGroup(), squad6HatesSquad1.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad2 != null) squad6.SetRelationshipWithGroup(squad2.GetRelationshipGroup(), squad6HatesSquad2.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad3 != null) squad6.SetRelationshipWithGroup(squad3.GetRelationshipGroup(), squad6HatesSquad3.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad4 != null) squad6.SetRelationshipWithGroup(squad4.GetRelationshipGroup(), squad6HatesSquad4.Checked ? Relationship.Hate : Relationship.Companion, true);
                        if (squad5 != null) squad6.SetRelationshipWithGroup(squad5.GetRelationshipGroup(), squad6HatesSquad5.Checked ? Relationship.Hate : Relationship.Companion, true);
                        foreach (KeyValuePair<string, Relationship> kvp in squad6CachedRelations)
                        {
                            int hash = Game.GenerateHash(kvp.Key);
                            squad6.SetRelationshipWithGroup(new RelationshipGroup(hash), kvp.Value);
                        }
                        squad6.Spawn();
                        squad6.CanExitVehicle = squad6CanExitVehicle.Checked;
                        squad6.LeaderReactsToEvents = squad6LeaderReacts.Checked;
                        squad6.Invincible = squad6GodMode.Checked;
                    }
                    else
                    {
                        squad6.RemovePedBlips();
                        squad6.RemoveVehicleBlip();
                        squad6 = null;
                    }
                }
                else
                {
                    foreach (Ped ped in squad6.Peds)
                    {
                        if (ped.Exists())
                        {
                            squad6.SetPedBlipSprite(ped, ped.IsInCombat ? BlipSprite.ShootingRange : BlipSprite.VIP);
                        }
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
            if (squad5 != null)
            {
                squad5.Dismiss();
                squad5 = null;
            }
            if (squad6 != null)
            {
                squad6.Dismiss();
                squad6 = null;
            }
        }
    }
}
