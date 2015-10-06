/*

       Drag Race
       by: ZyDevs

       This GTA Modification is no longer being developed, therefore I have released the source code.
       Everything below is needed for the mod to work, if you'd like to modify it.

       [Redistrubting]

       If you're interested in re-dist... contact me by skype: zy.co.sha.dy, we can talk about it there.
       If you choose to redistribute this without my permission, I will do my best to get the mod taken down.
       Once you redistribute, you will be required to give me credit, if not, I will do my best to get the mod taken down.



    Thanks, -ZyDevs

*/

using GTA; 
using GTA.Native;
using GTA.Math;
using System;
using System.IO;
using System.Timers;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using NativeUI;

namespace Drag_Race
{
    public class Drag_Race : Script
    {
        public bool active1 = false; 
        public bool active2 = false; 
        public bool active3 = false; 
        public bool active4 = false;
        Vehicle veh = Game.Player.Character.CurrentVehicle;
        Ped player = Game.Player.Character;
        Ped PedDriver;
        Vehicle PedVehicle;
        ScriptSettings Config;
        string activateKey;
        string disableKey;
        int maxSpeed;
        string captionStatus;
        bool waypointSet;
        VehicleHash selectedCarHash = VehicleHash.Buffalo;
        int custom1x;
        int _selectedCar = 1;
        int custom1y;
        bool showCoords = false;
        int custom1z;
        int custom2x;
        int custom2y;
        int custom2z;
        int custom3x;
        int custom3y;
        int custom3z;
        int customs3x;
        int customs3y;
        int customs3z;
        int customh3;
        Vector3 dest;
        // nativeui variables
        UIMenu mainMenu;
        UIMenuItem startOne;
        UIMenuItem startTwo;
        UIMenuItem startThree;
        UIMenuItem startFour;
        UIMenuListItem carSel;
        List<dynamic> cars;
        MenuPool menuPool = new MenuPool();
        void drawText(float x, float y, float scale, int font, int r, int g, int b, string text)
        {
            Function.Call(Hash.SET_TEXT_FONT, font);
            Function.Call(Hash.SET_TEXT_SCALE, scale, scale);
            Function.Call(Hash.SET_TEXT_COLOUR, r, g, b, 255);
            Function.Call(Hash.SET_TEXT_WRAP, 0.0, 1.0);
            Function.Call(Hash.SET_TEXT_CENTRE, false);
            Function.Call(Hash.SET_TEXT_DROPSHADOW, 2, 2, 0, 0, 0);
            Function.Call(Hash.SET_TEXT_EDGE, 1, 1, 1, 1, 205);
            Function.Call(Hash._SET_TEXT_ENTRY, "STRING");
            Function.Call(Hash._ADD_TEXT_COMPONENT_STRING, text);
            Function.Call(Hash._DRAW_TEXT, x, y);
        }
        public Drag_Race() // main function
        {
            var cars = new List<dynamic>
            {
                "Buffalo",
                "Bullet",
                "Adder",
                "Zentorno",
                "Banshee",
                "B-Type",
                "BATI",
                "Entity XF",
            };
            ParseSettings();
            Tick += OnTick;
            KeyDown += OnKeyDown;
            Interval = 1;
            mainMenu = new UIMenu("Drag Race", "~g~Main Menu");
            startOne = new UIMenuItem("Start Race: One!", "Starts race 1, the coordinates are changeable in the ini. \nX: " + custom1x + "\nY: " + custom1y + "\nZ: " + custom1z);
            startTwo = new UIMenuItem("Start Race: Two!", "Starts race 2, the coordinates are changeable in the ini. \nX: " + custom2x + "\nY: " + custom2y + "\nZ: " + custom2z);
            startThree = new UIMenuItem("Start Race: Custom Race!", "Starts race 3, ONLY USE IF YOU'VE CHANGED THE INI");
            startFour = new UIMenuItem("Start Race: Waypoint", "Starts waypoint race, will only start if you have a waypoint set.");
            carSel = new UIMenuListItem("Car", cars, 0, "Select your car.");
            mainMenu.AddItem(startOne);
            mainMenu.AddItem(startTwo);
            mainMenu.AddItem(startThree);
            mainMenu.AddItem(startFour);
            mainMenu.AddItem(carSel);
            mainMenu.RefreshIndex();
            menuPool.Add(mainMenu);
            mainMenu.OnItemSelect += ItemSelectHandler;
            mainMenu.OnListChange += OnListChange;
        }
        void ParseSettings()
        {
            try
            {
                Config = ScriptSettings.Load(@".\scripts\DragRace.ini");
                activateKey = Config.GetValue("Keys", "OpenMenu", "O");
                disableKey = Config.GetValue("Keys", "ManualEndRace", "NumPad1");
                maxSpeed = Config.GetValue<int>("OpponentSettings", "MaxSpeed", 200);
                custom1x = Config.GetValue<int>("Race1", "X", -1959);
                custom1y = Config.GetValue<int>("Race1", "Y", 600);
                custom1z = Config.GetValue<int>("Race1", "Z", 119);

                custom2x = Config.GetValue<int>("Race2", "X", 497);
                custom2y = Config.GetValue<int>("Race2", "Y", -1132);
                custom2z = Config.GetValue<int>("Race2", "Z", 29);

                customs3x = Config.GetValue<int>("Race3", "StartX", 0);
                customs3y = Config.GetValue<int>("Race3", "StartY", 0);
                customs3z = Config.GetValue<int>("Race3", "StartZ", 0);
                custom3x = Config.GetValue<int>("Race3", "EndX", 0);
                custom3y = Config.GetValue<int>("Race3", "EndY", 0);
                custom3z = Config.GetValue<int>("Race3", "EndZ", 0);
                customh3 = Config.GetValue<int>("Race3", "StartHeading", 270);
            }
            catch
            {
                UI.Notify("There was an error loading the configuration file (.ini) make sure it exists!");
            }
        }
        public void OnListChange(UIMenu sender, UIMenuListItem list, int index)
        {
            if (list == carSel)
            {
                switch(index)
                {
                    case 0:
                        _selectedCar = 0;
                        UI.Notify("~b~Buffalo");
                        break;
                    case 1:
                        _selectedCar = 1;
                        UI.Notify("~b~Bullet");

                        break;
                    case 2:
                        _selectedCar = 2;
                        UI.Notify("~b~Adder");

                        break;
                    case 3:
                        _selectedCar = 3;
                        UI.Notify("~b~Zentorno");
                        break;
                    case 4:
                        _selectedCar = 4;
                        UI.Notify("~b~Banshee");
                        break;
                    case 5:
                        _selectedCar = 5;
                        UI.Notify("~b~B-Type");
                        break;
                    case 6:
                        _selectedCar = 6;
                        UI.Notify("~b~BATI");
                        break;
                    case 7:
                        _selectedCar = 7;
                        UI.Notify("~o~ENTITY XF");
                        break;
                }
            }
        }
        public void ItemSelectHandler(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            if (sender == mainMenu)
            {
                switch(index)
                {
                    case 0:
                        if (player.IsInVehicle())
                        {
                            UI.Notify("Cannot Start Race In Vehicle.");
                        }
                        else
                        {
                            active1 = true;
                            setWaypoint(custom1x, custom1y);
                            waypointSet = true;
                            var playerPos = player.Position;

                            player.Position = new Vector3(-855, 159, 64);
                            player.Heading = 85;

                            Vector3 coords = player.GetOffsetInWorldCoords(playerPos);
                            Wait(210);
                            Vehicle playerVeh = World.CreateVehicle(selectedCarHash, Game.Player.Character.GetOffsetInWorldCoords(new Vector3(0, 5, 0)));
                            Wait(250);
                            Vehicle PedVehicle = World.CreateVehicle(selectedCarHash, Game.Player.Character.GetOffsetInWorldCoords(new Vector3(0, 10, 0)));
                            Function.Call(Hash.CREATE_RANDOM_PED_AS_DRIVER, PedVehicle, true);
                            Wait(500);
                            player.SetIntoVehicle(playerVeh, VehicleSeat.Driver);
                            Ped PedDriver = PedVehicle.GetPedOnSeat(VehicleSeat.Driver);
                            Vector3 waypoint1 = new Vector3(-1815, 96, 73);

                            DrivingStyle crazy = DrivingStyle.Rushed;
                            int crazy1 = Convert.ToInt32(crazy);
                            PedDriver.Task.DriveTo(PedVehicle, new Vector3(custom1x, custom1y, custom1z), 0, maxSpeed, crazy1);
                            //blip1.ShowRoute = true;
                        }
                        break;
                    case 1:

                        if (player.IsInVehicle())
                        {
                            UI.Notify("Cannot Start Race In Vehicle.");
                        }
                        else
                        {
                            active2 = true;
                            setWaypoint(custom2x, custom2y);
                            waypointSet = true;
                            Wait(210);
                            Vehicle playerVeh = World.CreateVehicle(selectedCarHash, new Vector3(-248, -1148, 22), 270);
                            Wait(250);
                            Vehicle PedVehicle = World.CreateVehicle(selectedCarHash, new Vector3(-241, -1143, 22), 270);
                            Function.Call(Hash.CREATE_RANDOM_PED_AS_DRIVER, PedVehicle, true);
                            Wait(500);
                            player.SetIntoVehicle(playerVeh, VehicleSeat.Driver);
                            Ped PedDriver = PedVehicle.GetPedOnSeat(VehicleSeat.Driver);
                            DrivingStyle crazy = DrivingStyle.Rushed;
                            int crazy1 = Convert.ToInt32(crazy);
                            PedDriver.Task.DriveTo(PedVehicle, new Vector3(custom2x, custom2y, custom2z), 0, maxSpeed, crazy1);
                            //blip1.ShowRoute = true;

                        }
                    
                        break;
                    case 2:
                        if (player.IsInVehicle())
                        {
                            UI.Notify("Cannot Start Race In Vehicle.");
                        }
                        else
                        {
                            active3 = true;
                            setWaypoint(custom3x, custom3y);
                            waypointSet = true;
                            Wait(210);
                            Vehicle playerVeh = World.CreateVehicle(selectedCarHash, new Vector3(customs3x, customs3y, customs3z), customh3);
                            Wait(250);
                            Vector3 offset = playerVeh.GetOffsetInWorldCoords(new Vector3(5, 0, 0));
                            Vehicle PedVehicle = World.CreateVehicle(selectedCarHash, offset, customh3);
                            Function.Call(Hash.CREATE_RANDOM_PED_AS_DRIVER, PedVehicle, true);
                            Wait(500);
                            player.SetIntoVehicle(playerVeh, VehicleSeat.Driver);
                            Ped PedDriver = PedVehicle.GetPedOnSeat(VehicleSeat.Driver);
                            DrivingStyle crazy = DrivingStyle.Rushed;
                            int crazy1 = Convert.ToInt32(crazy);
                            PedDriver.Task.DriveTo(PedVehicle, new Vector3(custom3x, custom3y, custom3z), 0, maxSpeed, crazy1);
                            //blip1.ShowRoute = true;

                        }
                        break;
                    case 3:
                        bool isReady = false;
                        if (player.IsInVehicle())
                        {
                            UI.Notify("Cannot Start Race In Vehicle.");
                        }
                        else
                        {
                            Vector3 dest = new Vector3();
                            if (Function.Call<bool>(Hash.IS_WAYPOINT_ACTIVE))
                            {
                                dest = GetWaypointCoords();
                                isReady = true;
                            }
                            else
                            {
                                UI.ShowSubtitle("Waypoint not set");
                            }
                            if (isReady)
                            {
                                active4 = true;
                                Wait(210);
                                Vehicle playerVeh = World.CreateVehicle(selectedCarHash, player.GetOffsetInWorldCoords(new Vector3(0, 5, 0)));
                                Wait(250);
                                Vector3 offset = playerVeh.GetOffsetInWorldCoords(new Vector3(5, 0, 0));
                                Vehicle PedVehicle = World.CreateVehicle(selectedCarHash, offset);
                                Function.Call(Hash.CREATE_RANDOM_PED_AS_DRIVER, PedVehicle, true);
                                Wait(500);
                                player.SetIntoVehicle(playerVeh, VehicleSeat.Driver);
                                Ped PedDriver = PedVehicle.GetPedOnSeat(VehicleSeat.Driver);
                                DrivingStyle crazy = DrivingStyle.Rushed;
                                int crazy1 = Convert.ToInt32(crazy);
                                PedDriver.Task.DriveTo(PedVehicle, dest, 0, maxSpeed, crazy1);
                            }
                        }
                        break;
                }
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            Keys activateKeyVal = (Keys)Enum.Parse(typeof(Keys), activateKey);
            Keys disableKeyVal = (Keys)Enum.Parse(typeof(Keys), disableKey);
           
            if (e.KeyCode == activateKeyVal)
            {
                mainMenu.Visible = !mainMenu.Visible;
            }
            if (e.KeyCode == Keys.C)
            {
                if (showCoords == false)
                {
                    showCoords = true;
                } else
                {
                    showCoords = false;
                }
                
            }
            if (e.KeyCode == disableKeyVal)
            {
                if (active1 == true)
                {
                    if (PedDriver.Exists())
                    {
                        active1 = false;
                        PedDriver.Task.CruiseWithVehicle(PedVehicle, 60);
                        UI.Notify("Race ended manually/mod disabled.");
                    }
                }
                if (active2 == true)
                {
                    if (PedDriver.Exists())
                    {
                        active2 = false;
                        PedDriver.Task.CruiseWithVehicle(PedVehicle, 60);
                        UI.Notify("Race ended manually/mod disabled.");
                    }
                }
                if (active3 == true)
                {
                    if (PedDriver.Exists())
                    {
                        active3 = false;
                        PedDriver.Task.CruiseWithVehicle(PedVehicle, 60);
                        UI.Notify("Race ended manually/mod disabled.");
                    }
                }
                if (active4 == true)
                {
                    if (PedDriver.Exists())
                    {
                        active4 = false;
                        PedDriver.Task.CruiseWithVehicle(PedVehicle, 60);
                        UI.Notify("Race ended manually/mod disabled.");
                    }
                }
            }


        }

        private static Vector3 GetWaypointCoords()
        {
            Vector3 wpVec = new Vector3();
            Blip wpBlip = new Blip(Function.Call<int>(Hash.GET_FIRST_BLIP_INFO_ID, 8));

            if (Function.Call<bool>(Hash.IS_WAYPOINT_ACTIVE))
            {
                wpVec = Function.Call<GTA.Math.Vector3>(Hash.GET_BLIP_COORDS, wpBlip);
				
            }
            else
            {
                UI.ShowSubtitle("Waypoint not set!");
            }
            return wpVec;
        }

        void setWaypoint(float coordx, float coordy)
        {
            if (waypointSet == false)
            {
                if (Function.Call<bool>(Hash.IS_WAYPOINT_ACTIVE) == true)
                {
                    Function.Call(Hash.SET_WAYPOINT_OFF);
                }
                else
                {
                    Function.Call(Hash.SET_NEW_WAYPOINT, coordx, coordy);
                    waypointSet = true;
                }
            }
        }

        private void OnTick(object sender, EventArgs e)
        {
            menuPool.ProcessMenus();
            switch(_selectedCar)
            {
                case 0:
                    selectedCarHash = VehicleHash.Buffalo;
                    break;
                case 1:
                    selectedCarHash = VehicleHash.Bullet;
                    break;
                case 2:
                    selectedCarHash = VehicleHash.Adder;
                    break;
                case 3:
                    selectedCarHash = VehicleHash.Zentorno;
                    break;
                case 4:
                    selectedCarHash = VehicleHash.Banshee;
                    break;
                case 5:
                    selectedCarHash = VehicleHash.BType;
                    break;
                case 6:
                    selectedCarHash = VehicleHash.Bati2;
                    break;
                case 7:
                    selectedCarHash = VehicleHash.EntityXF;
                    break;
            }
            
            
            Vector3 playerPos = player.Position;
            if (active1 == true)
            {
                float distance = World.GetDistance(player.Position, new Vector3(custom1x, custom1y, custom1z));
                
                if (distance <= 19)
                {
                    UI.Notify("Race ~y~Over! ~s~Stay tuned for more!");
                    active1 = false;
                    waypointSet = false;
                }
                if (player.IsInVehicle() == false)
                {
                    UI.ShowSubtitle("~g~Race will start when you enter the EMPTY vehicle.");
                }
                else
                {
                    UI.ShowSubtitle("");
                }
            }
            else
            {
                waypointSet = false;

            }
            if (active2 == true)
            {
                float distance = World.GetDistance(player.Position, new Vector3(custom2x, custom2y, custom2z));

                if (distance <= 19)
                {
                    UI.Notify("Race ~y~Over! ~s~Stay tuned for more!");
                    active2 = false;
                    waypointSet = false;
                }
                if (player.IsInVehicle() == false)
                {
                    UI.ShowSubtitle("~g~Race will start when you enter the EMPTY vehicle.");
                }
                else
                {
                    UI.ShowSubtitle("");
                }
            }
            else
            {
                waypointSet = false;

            }
            if (active3 == true)
            {
                float distance = World.GetDistance(player.Position, new Vector3(custom2x, custom2y, custom2z));

                if (distance <= 19)
                {
                    UI.Notify("Race ~y~Over! ~s~Stay tuned for more!");
                    active3 = false;
                    waypointSet = false;
                }
                if (player.IsInVehicle() == false)
                {
                    UI.ShowSubtitle("~g~Race will start when you enter the EMPTY vehicle.");
                }
                else
                {
                    UI.ShowSubtitle("");
                }
            }
            else
            {
                waypointSet = false;

            }
            if (active4 == true)
            {
                float distance = World.GetDistance(player.Position, dest);

                if (distance <= 19)
                {
                    UI.Notify("Race ~y~Over! ~s~Stay tuned for more!");
                    active4 = false;
                }
                if (player.IsInVehicle() == false)
                {
                    UI.ShowSubtitle("~g~Race will start when you enter the EMPTY vehicle.");
                }
                else
                {
                    UI.ShowSubtitle("");
                }
            }
            else
            {

            }
            if (showCoords)
            {
                UI.ShowSubtitle("X: " + player.Position.X + " Y: " + player.Position.Y + " Z: " + player.Position.Z + " Heading: " + player.Heading);
            }

        }

       
        }
    }
    
