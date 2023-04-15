using System;
using System.Collections.Generic;
using Map.Signs;
using TargetsSystem.Points;
using UI.Search.Options;

namespace Navigation
{
    public struct PointInfo : IOptionInfo
    {
        private const string DefaultName = "Объект";
        
        public string Name { get; }
        public Address Address { get; }
        public bool NameIsRoomId => Address.HasRoomId && Name == Address.RoomId;

        
        public PointInfo(Point point, Address address)
        {
            if (point.SignCreator.SignPreset.HasName)
                Name = point.SignCreator.SignPreset.Name;
            else if (point is AccessibleRoom accessibleRoom)
                Name = accessibleRoom.Id;
            else
                Name = DefaultName;
            
            Address = address;
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
            {PointType.Buffet, "буфет"}
        };

        public int Floor => FloorIndex + 1;
        public int FloorIndex { get; }
        public string BlockName { get; }
        public string RoomId { get; }
        public PointType PointType { get; }
        public int PointNumber { get; }

        public bool HasRoomId => RoomId != String.Empty;


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
                result += $", {TypesNames[PointType]}, {PointNumber}";
            
            if (RoomId != string.Empty)
                result += $", ауд. {RoomId}";

            return result;
        }
    }
}
