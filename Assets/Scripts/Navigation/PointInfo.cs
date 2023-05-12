using System;
using System.Collections.Generic;
using Map.Signs;
using Newtonsoft.Json;
using TargetsSystem.Points;
using UI.Search.Options;

namespace Navigation
{
    public struct PointInfo : IOptionInfo
    {
        private const string DefaultName = "Объект";
        
        [JsonProperty("n")] public string Name { get; private set; }
        [JsonProperty("a")] public Address Address { get; private set; }
        [JsonProperty("iwp")] public bool IsWayPoint { get; private set; }
        [JsonIgnore] public bool NameIsRoomId => Address.HasRoomId && Name == Address.RoomId;

        
        public PointInfo(Point point, Address address)
        {
            if (point.SignCreator.SignPreset.HasName)
                Name = point.SignCreator.SignPreset.Name;
            else if (point is AccessibleRoom accessibleRoom)
                Name = accessibleRoom.Id;
            else
                Name = DefaultName;
            
            Address = address;
            IsWayPoint = point.IsWayPoint;
        }
    }
    
    public struct Address
    {
        private static readonly Dictionary<PointType, string> TypesNames = new Dictionary<PointType, string>()
        {
            {PointType.None, String.Empty},
            {PointType.ManToilet, "мужской туалет"},
            {PointType.WomanToilet, "женский туалет"},
            {PointType.Stairs, "лестница"},
            {PointType.Elevator, "лифт"},
            {PointType.ATM, "банкомат"},
            {PointType.Library, "читальный зал"},
            {PointType.Print, "пункт печати"},
            {PointType.Buffet, "буфет"},
            {PointType.Marker, "маркер"}
        };

        [JsonProperty("fi")] public int FloorIndex { get; private set; }
        [JsonProperty("bn")] public string BlockName { get; private set;}
        [JsonProperty("r")] public string RoomId { get; private set;}
        [JsonProperty("t")] public PointType PointType { get; private set;}
        [JsonProperty("pn")] public int PointNumber { get; private set;}

        [JsonIgnore] public int Floor => FloorIndex + 1;
        [JsonIgnore] public bool HasRoomId => RoomId != String.Empty;


        public Address(int floorIndex, string blockName)
        {
            FloorIndex = floorIndex;
            BlockName = blockName;
            RoomId = String.Empty;
            PointType = PointType.None;
            PointNumber = 0;
        }
        
        public Address(int floorIndex, string blockName, string roomId) : this(floorIndex, blockName)
        {
            RoomId = roomId;
        }
        
        public Address(int floorIndex, string blockName, string roomId, PointType pointType, int pointNumber) : this(floorIndex, blockName, roomId)
        {
            PointType = pointType;
            PointNumber = pointNumber;
        }
        

        public override string ToString()
        {
            string result = $"Этаж {Floor}, {BlockName}";

            if (PointType != PointType.None)
                result += $", {TypesNames[PointType]} {PointNumber}";
            
            if (RoomId != string.Empty && 
                PointType != PointType.ManToilet && 
                PointType != PointType.WomanToilet)
                result += $", <nobr>ауд. {RoomId}</nobr>";

            return result;
        }
    }
}
