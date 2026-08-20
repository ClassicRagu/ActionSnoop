using System.Collections.Generic;

namespace ActionViewer.Models
{
    public class PJobMappings
    {
        public static string[] pJobList = [
            "Freelancer",
            "Knight",
            "Berserker",
            "Monk",
            "Ranger",
            "Samurai",
            "Bard",
            "Geomancer",
            "Time Mage",
            "Cannoneer",
            "Chemist",
            "Oracle",
            "Thief",
            "Mystic Knight",
            "Gladiator",
            "Dancer",
            "Ninja",
            "White Mage",
            "Black Mage",
            "Dragoon",
            "Summoner",
            "Blue Mage",
            "Red Mage",
            "Necromancer"
            ];

        public static int[] levelReq = [
            24,
            6,
            3,
            6,
            6,
            5,
            4,
            5,
            5,
            6,
            4,
            5,
            6,
            4,
            4,
            4,
            6,
            5,
            5,
            4,
            5,
            3,
            6,
            5
        ];
        public static Dictionary<uint, int> pJobDict = new Dictionary<uint, int>{
            {0, 0},
            {4242, 0}, // Freelancer
            {4358, 1}, // Knight
            {4359, 2}, // Berserker
            {4360, 3}, // Monk
            {4361, 4}, // Ranger
            {4362, 5}, // Samurai
            {4363, 6}, // Bard
            {4364, 7}, // Geomancer
            {4365, 8}, // Time Mage
            {4366, 9}, // Cannoneer
            {4367, 10}, // Chemist
            {4368, 11}, // Oracle
            {4369, 12}, // Thief
            {4803, 13}, // Mystic Knight
            {4804, 14}, // Gladiator
            {4805, 15}, // Dancer
            {5328, 16}, // Ninja
            {5329, 17}, // White Mage
            {5330, 18}, // Black Mage
            {5331, 19}, // Dragoon
            {5332, 20}, // Summoner
            {5333, 21}, // Blue Mage
            {5334, 22}, // Red Mage
            {5335, 23}  // Necromancer 
        };
    }
}