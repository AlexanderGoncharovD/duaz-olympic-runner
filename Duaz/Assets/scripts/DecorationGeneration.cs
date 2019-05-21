// Скрипт вешается на родительский объект локации(начало иерархии объекта)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Код отвечает за генерацию декоративных объектов на сцене, вдоль беговой дорожки
* Dec - Decoration*/
public class DecorationGeneration : MonoBehaviour
{
    // Все спрайты используемых декораций
    [Header("Decoration sprites")]
    public Sprite[] TreeDec = new Sprite[5]; // массив спрайтов деревьев
    public Sprite[] GroundRockDec = new Sprite[4], // спрайты камня на поверхности
        UndergroundRockDec = new Sprite[4], // спрайты камня под землей
        BushDec = new Sprite[3], // спрайты кустов
        GrassDec = new Sprite[1], // спрайты травы
        RubbishDec = new Sprite[5]; // спрайты различного мусора под землей

    // параметры управления генерацией объектов
    [Header("Chunk settings")]
    public Transform Chunk; // Чанк - центральная точка для генерации декорации в пределах одного чанка
    public Vector2 SizeChunk = new Vector2(3.0f, 0.235f); // Размер одного чанка
    public Vector2Int ChunkGrid = new Vector2Int(3, 3); // количество клеток в одном чанке по х и по у
    Vector2 CellChunk; // координаты одной клетки (результат деления sizeChunk на ChunkGrid)
    string AvailableCells; // Все доступные для генерации ячейки
    string PossibleCells; // Все оставшиеся доступные ячейки для генерации декораций в пределах одного чанка

    // параметры управления генерацией объектов
    [Header("Generation settings")]
    public float valueGenerateTree; // Шанс генерации дерева (в процентах)
    public float valueGenerateRock, // Шанс генерации наземного камня (в процентах)
        valueGenerateBush, // Шанс генерации куста (в процентах)
        valueGenerateGrass; // Шанс генерации травы (в процентах)
    int NumberOfChanks; // Количество чанков на всю длину

    // параметры управления генерацией объектов
    [Header("Other")]
    public float LengthLocation = 18; // Длина всей локации
    public int ZeroLayer; // нулевой слой, начало отсчета для слоев новых объектовs
    public GameObject prefabEmptySprite; // пустая болванка для спрайта
    public Transform ParentDecoration; // Родительский объект для декораций
    GameObject[] LayersGO; // группировка декорация по номеру слоя

    // Use this for initialization
    void Start ()
    {
        LengthLocation = this.GetComponent<LocationGeneration>().LengthLocation * 3;
        ChunkCalculations ();
    }

    /*Выполнение всех предварительных расчетов касательно чанка*/
    void ChunkCalculations ()
    {
        NumberOfChanks = Mathf.CeilToInt(LengthLocation / SizeChunk.x); // Количество чанков на всю длину
        CellChunk = new Vector2(SizeChunk.x / ChunkGrid.x - (SizeChunk.x / ChunkGrid.x) / 2,
                                SizeChunk.y / ChunkGrid.y - (SizeChunk.y / ChunkGrid.y) / 2); // Расчет координат одной клетки
        // 
        for (int row = 1; row <= ChunkGrid.x; row++)
        {
            for (int col = 1; col <= ChunkGrid.y; col++)
            {
                AvailableCells += row + "" + col + ":";
            }
        }

        // Создание пустышек для группировки декорация по номеру слоя
        LayersGO = new GameObject[ChunkGrid.y];
        for (int i = 1; i <= ChunkGrid.y; i++)
        {
            LayersGO[i - 1] = new GameObject();
            LayersGO[i - 1].name = "layer";
            LayersGO[i - 1].tag = "layer";
            LayersGO[i - 1].transform.parent = ParentDecoration;
        }
        ChunkGeneration ();
    }

    /*Генерация объектов декорации в пределах одного чанка на верхней траве над дорогой*/
    void ChunkGeneration ()
    {
        GameObject newDecoration; // новая генерируема декорация

        for (int i = 0; i < NumberOfChanks; i++)
        {
            // Клетки для спавна декораций
            PossibleCells = AvailableCells;

            // Генерация дерева
            if (Random.Range(0.0f, 1.0f) < valueGenerateTree)
            {
                newDecoration = CreateDecoration(TreeDec[Random.Range(0, TreeDec.Length)]); // генерация дерева с случайным спрайтом
                newDecoration.name = "tree_" + i;
                //newDecoration.transform.parent = ParentDecoration;
            }

            // генерация камней
            if (Random.Range(0.0f, 1.0f) < valueGenerateRock)
            {
                for (int k = 0; k <= Random.Range(0, 2); k++)
                {
                    newDecoration = CreateDecoration(GroundRockDec[Random.Range(0, GroundRockDec.Length)]); // генерация наземного камня с случайным спрайтом
                    newDecoration.name = "groundRock_" + i + "_" + k ;
                    //newDecoration.transform.parent = ParentDecoration;
                }
            }

            // генерация кустов
            if (Random.Range(0.0f, 1.0f) < valueGenerateBush)
            {
                newDecoration = CreateDecoration(BushDec[Random.Range(0, BushDec.Length)]); // генерация куста с случайным спрайтом
                newDecoration.name = "bush_" + i;
                //newDecoration.transform.parent = ParentDecoration;
            }

            // генерация травы
            if (Random.Range(0.0f, 1.0f) < valueGenerateGrass)
            {
                newDecoration = CreateDecoration(GrassDec[Random.Range(0, GrassDec.Length)]); // генерация травы с случайным спрайтом
                newDecoration.name = "grass_" + i;
                //newDecoration.transform.parent = ParentDecoration;
            }
            Chunk.position = new Vector3(Chunk.position.x + SizeChunk.x/2, Chunk.position.y, Chunk.position.z);
        }
    }

    /*Генерация объекта декорации с получаемым спрайтом и присвоением номера слоя*/
    GameObject CreateDecoration (Sprite sprite)
    {
        string[] AllCells = PossibleCells.Split(char.Parse(":")); // разделение доступных ячеек
        int rndIndexCell = Random.Range(0, AllCells.Length-1); // Выбор случайной ячейки из доступных

        /* Ячейка в чанке, в которой будет сгенерирована декорация
         * пример получаемой строки: "25", где 2-номер строки, 5-номер столбца
         * Vector2Int(удаление последнего символа из строки, удаление первого символа из строки) - определение номера строки и номера столбца*/
        Vector2Int cell = new Vector2Int(int.Parse(AllCells[rndIndexCell].
            Substring(0, AllCells[1].Length - 1)),
                                    int.Parse(AllCells[rndIndexCell].Substring(1)));
        int layer = cell.y; // Определение слоя декорации

        // опредление позиции для генерации декорации
        Vector3 position = Chunk.position + new Vector3(cell.x * CellChunk.x, cell.y * CellChunk.y, 0);
        GameObject newObject = Instantiate(prefabEmptySprite, position, Quaternion.identity);
        newObject.GetComponent<SpriteRenderer>().sprite = sprite;
        newObject.GetComponent<SpriteRenderer>().sortingOrder = ZeroLayer - layer;
        newObject.transform.parent = LayersGO[layer - 1].transform;

        // Удаление использованной ячейки из списка оставшихся доступных ячеек
        PossibleCells = "";
        for (int i = 0; i < AllCells.Length; i++)
        {
            if (i == rndIndexCell || AllCells[i] == "" || AllCells[i].Substring(0, AllCells[1].Length - 1) == cell.x+ ""/*исключает столбец*/) 
            {
            }
            else
            {
                PossibleCells += AllCells[i] + ":";
            }
        }
        return newObject;
    }
}
