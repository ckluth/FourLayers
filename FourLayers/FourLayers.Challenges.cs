using System.Collections.Generic;

namespace FourLayers;

public static partial class FourLayers
{
    public static List<Challenge> Challenges { get; set; }

    private static void InitChallenges()
    {
        FourLayers.Challenges =
        [
            new Challenge
            {
                Number = 1,

                Field = new byte[,]
                {
                    {0,1,1,0},
                    {1,1,1,1},
                    {1,1,1,1},
                    {0,1,1,0},
                },

                MaxMoves = 8,
            },

            new Challenge
            {
                Number = 2,

                Field = new byte[,]
                {
                    {1,0,0,0},
                    {1,0,1,1},
                    {1,1,1,1},
                    {1,1,1,1},
                },

                MaxMoves = 4,
            },

            new Challenge
            {
                Number = 3,

                Field = new byte[,]
                {
                    {1,1,1,0},
                    {1,1,1,0},
                    {0,1,1,1},
                    {1,0,1,1},
                },

                MaxMoves = 5,
            },


            new Challenge
            {
                Number = 4,

                Field = new byte[,]
                {
                    {1,1,0,1},
                    {1,1,1,0},
                    {1,0,1,1},
                    {1,1,0,1},
                },

                MaxMoves = 3,
            },

            new Challenge
            {
                Number = 5,

                Field = new byte[,]
                {
                    {2,2,2,2,2,1},
                    {2,1,1,1,1,2},
                    {2,1,0,0,1,2},
                    {2,1,0,0,1,2},
                    {2,2,1,1,1,2},
                    {2,2,2,2,2,2},
                },

                MaxMoves = 3,
            },

            new Challenge
            {
                Number = 6,

                Field = new byte[,]
                {
                    {2,2,1,2,2,2},
                    {2,1,1,2,1,2},
                    {2,1,0,0,1,2},
                    {2,1,0,0,1,2},
                    {2,1,1,1,1,2},
                    {2,2,2,2,2,2},
                },

                MaxMoves = 3,
            },


            new Challenge
            {
                Number = 7,

                Field = new byte[,]
                {
                    {2,2,1,2,2,2},
                    {2,1,2,0,1,2},
                    {2,1,1,0,1,2},
                    {2,1,0,1,1,2},
                    {2,1,0,2,1,2},
                    {2,1,2,2,2,2},
                },

                MaxMoves = 4,
            },

            new Challenge
            {
                Number = 8,

                Field = new byte[,]
                {
                    {1,1,0,1},
                    {0,1,1,1},
                    {1,1,1,0},
                    {1,0,1,1},
                },
                MaxMoves = 5,
            },

            new Challenge
            {
                Number = 9,

                Field = new byte[,]
                {
                    {3,3,3,3,3,3,3,3},
                    {3,2,2,2,2,2,2,3},
                    {3,2,1,1,1,1,2,3},
                    {3,2,1,0,0,1,3,2},
                    {3,2,1,0,0,1,3,2},
                    {3,2,1,1,1,1,3,2},
                    {3,2,2,2,2,2,2,3},
                    {3,3,3,3,3,3,3,3},

                },
                MaxMoves = 9,
            },

            new Challenge
            {
                Number = 10,

                Field = new byte[,]
                {
                    {2,2,2,2,2,2},
                    {2,1,2,1,2,2},
                    {2,2,1,0,1,1},
                    {1,1,0,0,2,2},
                    {2,1,0,1,1,2},
                    {2,2,1,2,1,2},

                },
                MaxMoves = 4,
            },

            new Challenge
            {
                Number = 11,

                Field = new byte[,]
                {
                    {2,2,2,2,1,2},
                    {2,1,1,1,2,2},
                    {2,1,0,0,1,2},
                    {2,1,0,0,1,2},
                    {1,2,1,1,1,2},
                    {2,2,2,2,2,2},

                },
                MaxMoves = 5,
            },


            new Challenge
            {
                Number = 12,

                Field = new byte[,]
                {
                    {3,3,3,3,3,3,3,3},
                    {3,2,2,2,2,2,2,3},
                    {3,2,1,1,1,1,2,3},
                    {3,2,1,1,1,0,2,3},
                    {3,2,0,0,0,1,2,3},
                    {3,2,1,1,1,1,2,3},
                    {3,2,2,2,2,2,2,3},
                    {3,3,3,3,3,3,3,3},
                },
                MaxMoves = 6,
            },


            new Challenge
            {
                Number = 13,

                Field = new byte[,]
                {
                    {2,2,2,2,2,2},
                    {2,2,1,1,1,2},
                    {1,1,0,0,2,1},
                    {2,2,0,0,2,1},
                    {2,1,1,1,1,2},
                    {2,1,2,2,2,2},

                },
                MaxMoves = 7,
            },

            new Challenge
            {
                Number = 14,

                Field = new byte[,]
                {
                    {2,2,2,2,2,2},
                    {2,1,2,0,1,2},
                    {2,2,1,0,1,1},
                    {2,1,0,0,1,2},
                    {2,1,1,2,1,1},
                    {2,2,2,1,2,2},

                },
                MaxMoves = 9,
            },


            new Challenge
            {
                Number = 15,

                Field = new byte[,]
                {
                    {3,3,3,3,2,3,3,3},
                    {3,2,2,2,1,3,2,3},
                    {3,2,1,1,2,1,2,3},
                    {3,2,1,0,0,1,2,3},
                    {3,2,1,0,0,1,2,3},
                    {3,2,1,1,1,1,2,3},
                    {2,2,2,3,2,2,3,3},
                    {3,3,3,3,2,3,3,3},
                },
                MaxMoves = 7,
            },

            new Challenge
            {
                Number = 16,

                Field = new byte[,]
                {
                    {3,3,3,3,3,3,3,3},
                    {3,2,2,2,2,2,2,3},
                    {3,2,1,1,0,1,2,3},
                    {3,2,1,0,1,1,2,3},
                    {3,2,1,1,1,0,2,3},
                    {3,2,0,1,1,1,2,3},
                    {3,2,2,2,2,2,2,3},
                    {3,3,3,3,3,3,3,3},
                },
                MaxMoves = 8,
            },

            new Challenge
            {
                Number = 17,

                Field = new byte[,]
                {
                    {2,2,2,1,2,2},
                    {2,1,1,0,2,2},
                    {2,1,0,1,2,2},
                    {2,1,0,1,0,1},
                    {1,1,2,1,1,2},
                    {2,2,2,2,2,2},

                },
                MaxMoves = 6,
            },

            new Challenge
            {
                Number = 18,

                Field = new byte[,]
                {
                    {3,3,3,3,3,3,3,3},
                    {3,1,3,2,2,2,2,3},
                    {3,2,2,1,1,1,2,3},
                    {3,3,1,1,0,0,1,2},
                    {3,2,2,0,0,1,2,3},
                    {3,2,1,1,1,2,2,3},
                    {3,2,2,2,2,2,2,1},
                    {3,3,3,3,3,3,3,3},
                },
                MaxMoves = 11,
            },
        ];
    }
}
