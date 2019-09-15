using System;
using TwoPhaseSolver;

namespace SolverTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // Just solve a random cube with some pattern.
            Cube c = Move.None.apply(new Cube());
            //Console.WriteLine("Corner 0:" + c.corners[0].pos + "-" + c.corners[0].orient);
            //Console.WriteLine("Corner 1:" + c.corners[1].pos + "-" + c.corners[1].orient);
            //Console.WriteLine("Corner 2:" + c.corners[2].pos + "-" + c.corners[2].orient);
            //Console.WriteLine("Corner 3:" + c.corners[3].pos + "-" + c.corners[3].orient);
            //Console.WriteLine("Corner 4:" + c.corners[4].pos + "-" + c.corners[4].orient);
            //Console.WriteLine("Corner 5:" + c.corners[5].pos + "-" + c.corners[5].orient);
            //Console.WriteLine("Corner 6:" + c.corners[6].pos + "-" + c.corners[6].orient);
            //Console.WriteLine("Corner 7:" + c.corners[7].pos + "-" + c.corners[7].orient);

            //Console.WriteLine("Edge 0:" + c.edges[0].pos + "-" + c.edges[0].orient);
            //Console.WriteLine("Edge 1:" + c.edges[1].pos + "-" + c.edges[1].orient);
            //Console.WriteLine("Edge 2:" + c.edges[2].pos + "-" + c.edges[2].orient);
            //Console.WriteLine("Edge 3:" + c.edges[3].pos + "-" + c.edges[3].orient);
            //Console.WriteLine("Edge 4:" + c.edges[4].pos + "-" + c.edges[4].orient);
            //Console.WriteLine("Edge 5:" + c.edges[5].pos + "-" + c.edges[5].orient);
            //Console.WriteLine("Edge 6:" + c.edges[6].pos + "-" + c.edges[6].orient);
            //Console.WriteLine("Edge 7:" + c.edges[7].pos + "-" + c.edges[7].orient);
            //Console.WriteLine("Edge 8:" + c.edges[8].pos + "-" + c.edges[8].orient);
            //Console.WriteLine("Edge 9:" + c.edges[9].pos + "-" + c.edges[9].orient);
            //Console.WriteLine("Edge 10:" + c.edges[10].pos + "-" + c.edges[10].orient);
            //Console.WriteLine("Edge 11:" + c.edges[11].pos + "-" + c.edges[11].orient);
            Move pattern;
            c.corners = new Cubie[8]
            {
                new Cubie(4, 1), new Cubie(0, 2), new Cubie(2, 0), new Cubie(3, 0),
                new Cubie(5, 2), new Cubie(1, 1), new Cubie(6, 0), new Cubie(7, 0)
            };

            c.edges = new Cubie[12]
            {
                new Cubie(0, 0), new Cubie(8, 1), new Cubie(2, 0), new Cubie(3, 0),
                new Cubie(4, 0), new Cubie(9, 1), new Cubie(6, 0), new Cubie(7, 0),
                new Cubie(5, 1), new Cubie(1, 1), new Cubie(10, 0), new Cubie(11, 0)
            };
            // BEST. RANDOM. GEN. EVER. (actually not that bad,
            // since you'd have to time yourself with 100nanosecond precision
            if ((DateTime.Now.Ticks & 1) == 0)
            {
                pattern = Move.None;
                Console.WriteLine("No pattern this time...");
            }
            else
            {
                pattern = Move.randmove(20);
                Console.WriteLine("Pattern is {0}", pattern);
            }

            // Do the actual solve while printing what is happening
            Search.patternSolve(c, pattern, 22, printInfo: true);

            // End
            Console.Write("Press any key to continue...");
            Console.Read();
        }
    }
}
