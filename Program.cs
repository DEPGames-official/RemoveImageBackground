using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

class Program
{
    static void Main(string[] args)
    {
        string inputFilePath = "LogoEdited.png";  // Path to the input image
        string outputFilePath = "OutputEditedLogo.png"; // Path to save the output image

        using (Image<Rgba32> image = Image.Load<Rgba32>(inputFilePath))
        {
            int width = image.Width;
            int height = image.Height;
            bool[,] visited = new bool[width, height];

            Queue<(int, int)> queue = new Queue<(int, int)>();
            queue.Enqueue((0, 0)); // Start from the top-left corner
            visited[0, 0] = true;

            int processedPixels = 0;
            int totalPixels = width * height;

            while (queue.Count > 0)
            {
                var (x, y) = queue.Dequeue();
                var pixel = image[x, y];

                if (IsColorInWhiteOrLightGrayRange(pixel, 230, 50))
                {
                    // Set the pixel to be transparent
                    image[x, y] = new Rgba32(255, 255, 255, 0);

                    // Enqueue neighbors
                    foreach (var neighbor in GetNeighbors(x, y, width, height))
                    {
                        var (nx, ny) = neighbor;
                        if (!visited[nx, ny])
                        {
                            visited[nx, ny] = true;
                            queue.Enqueue((nx, ny));
                        }
                    }
                }

                processedPixels++;
                if (processedPixels % 1000 == 0)
                {
                    Console.WriteLine($"Processed {processedPixels} of {totalPixels} pixels ({(processedPixels / (float)totalPixels) * 100:F2}%).");
                }
            }

            image.Save(outputFilePath);
        }

        Console.WriteLine("Processing complete. Image saved as " + outputFilePath);
    }

    static IEnumerable<(int, int)> GetNeighbors(int x, int y, int width, int height)
    {
        // 8 possible directions (left, right, up, down, and diagonals)
        int[] dx = { -1, 1, 0, 0, -1, -1, 1, 1 };
        int[] dy = { 0, 0, -1, 1, -1, 1, -1, 1 };

        for (int i = 0; i < 8; i++)
        {
            int nx = x + dx[i];
            int ny = y + dy[i];

            if (nx >= 0 && ny >= 0 && nx < width && ny < height)
            {
                yield return (nx, ny);
            }
        }
    }

    static bool IsColorInWhiteOrLightGrayRange(Rgba32 color, int minGrayValue, int tolerance)
    {
        // Calculate the average value of RGB
        int avg = (color.R + color.G + color.B) / 3;

        // Check if the average value is above the minGrayValue and within the tolerance range
        return avg >= minGrayValue &&
               Math.Abs(color.R - avg) <= tolerance &&
               Math.Abs(color.G - avg) <= tolerance &&
               Math.Abs(color.B - avg) <= tolerance;
    }
}
