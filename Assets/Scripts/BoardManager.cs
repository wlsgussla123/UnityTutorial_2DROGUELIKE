using UnityEngine; 
using System; // serialisable attribute를 사용할 수 있다. 
using System.Collections.Generic; //Allows us to use Lists.
using Random = UnityEngine.Random; //Tells Random to use the Unity Engine random number generator.

// serialisable attribute :  using serialisable just allow us to modify how variables will appear in the inspector and in the editor and to show and hide them using a fold out.


public class BoardManager : MonoBehaviour {

    // Using Serializable allows us to embed a class with sub properties in the inspector.
    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    // meaning 8 x 8 games
    public int columns = 8;
    public int rows = 8;
    public Count wallCount = new Count(5, 9); // 몇개의 벽을 나타낼 것인지(각 레벨마다), 레벨 당 최소 5 최대 9개의 벽들
    public Count foodCount = new Count(1, 5); // 음식 개수
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;

    private Transform boardHolder; // keep the hierachy clean becasue we're going to be spawning a lot of game objects (Board holder의 자식으로 할 것이다)
    private List<Vector3> gridPositions = new List<Vector3>(); // track all of the possible positions on our game board and to keep track of whether an object has been spawned in that position or not.

    // Initilization function
    void InitialiseList()
    {
        gridPositions.Clear();

        // Field를 채우기 위한 Loop (to place Walls, Enemies or Pickups
        for(int x = 1; x < columns - 1; x++)
        {
            for(int y = 1; y < rows-1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    // setup the outer wall and the floor (background) of our game board.
    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform; 

        // (-1,-1) ~ (8,8) : 즉, 외벽 포함해서 
        for(int x = -1; x < columns + 1; x++)
        {
            for(int y = -1; y < columns + 1; y++)
            {
                // choose a floor tile at random from our array floor tiles and prepare instantiate it.
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];

                // if there're in one of the outer wall positions, (-1,이거나 columns or rows 와 같다면 외벽이 위치할 좌표이므로)
                if(x == -1 || x == columns || y == -1 || y == rows)
                {
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                }

                // 일단, 인스턴스화가 준비되면 toInstantiate 변수를 통하여 선택된 prefabs를 인스턴스화 한다.
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(boardHolder); // 부모를 boardHolder로 설정
            }
        }
    }

    // return a random position from our list gridPositions.
    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];

        gridPositions.RemoveAt(randomIndex); // 사용한 randomIndex 삭제 (중복으로 스폰이 겹치지 않게 하기 위하여)

        return randomPosition; // to spawn our object in a random location.
    }

    // LayoutObjectAtRandom accepts an array of game objects to choose from along with a minumum and maximum range for the number of objects to create.
    void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1); // choose random number to instantiate within the mimum and maximum limits

        for(int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition(); // choose a random position
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)]; // choose a random tile from tileArray.

            // Instantiate tileChoice at the position returned by RnadomPosition with no change in rotation
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

	public void SetupScene(int level)
    {
        // create the outer walls and floor
        BoardSetup();
        // Reset our list of gridpositions.
        InitialiseList();

        // Instantiate a random number of food tiles based on minimum and maximum
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);

        int enemyCount = (int)Mathf.Log(level, 2f); // 레벨에 따라 적의 수가 달라진다. ex) 1레벨엔 2마리, 2레벨엔 4마리...

        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount); // 최소 최대 없이 레벨에 따라서 적의 수를 정하게
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity); //    
    }
}
