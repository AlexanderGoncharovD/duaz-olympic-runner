using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Скрипт отвечает за генерацию объектов локации: дорогу, препятствия и декорации*/

/*Настройки генерации элементов дороги*/
[System.Serializable]
public struct Location
{
#if UNITY_EDITOR
    public string Name;
#endif
    public GameObject Prefab; // Ссылка на генерируемый объект
    public float Length; // Длмна данного элемента локации
    [Range(0.0f, 1.0f)]
    public float TotalPercentage; // Общий процент - количество генеририруемое на протяжении всей дороги
    [Range(0.0f, 1.0f)]
    public float Chance; // Индивидульный процент - шанс, при котором будет сгенерирована этот элемент локации
    public float Number;
    public float RemoveDistance; // Расстояние от центра, в радиусе которого не генерируются элементы окружения
}

/*Настройки генрации препятствий*/
[System.Serializable]
public struct Barrier
{
#if UNITY_EDITOR
    public string Name;
#endif
    public GameObject Prefab; // Ссылка на генерируемый объект
    public Vector2 RangeGenerationDistance;  // Дистанция необходимая от предыдущего перпятствия
    [Range(.0f, 1f)]
    public float Chance; // Индивидульный процент - шанс, при котором будет сгенерирована это перпятствие
    public float RemovealRadiusDecoretion; // Радиус, в котором удалаются декорации окружения
}

/*Настройки генерации дальних декораций*/
[System.Serializable]
public struct Decoration{
    public string Name;
    public Sprite[] Object; // Ссылка на все объекты одного вида
    [Range(.0f, 1f)]
    public float Chance; // Вероятность, с которой будет сгенерирован объект
    public Vector2 Number; // Количество генерируемых объектов в пределах одного чанка
    public Vector2 RandomRotation; // Случайный поворот
}
/*Настройки генерации чанков с декорациями*/
[System.Serializable]
public struct ChunkDicoretionGenerator
{
    public string Name;
    public Decoration[] Decorations; // Декорации, которые могут быть в этом чанке
    public Vector2Int Cells; // Количестов строк и столбцов в чанке
    public Transform Chunk; // Положение чанка, размеры чанка задаются в Scale
    public Transform[] CellChunk; // Клетки чанка
    public string AvailableCells; // Все доступные для генерации ячейки
    public string PossibleCells; // Все оставшиеся доступные ячейки для генерации декораций в пределах одного чанка
    public bool isGenerateDecorationBehindBarriers; // не генерировать декорации за препятсвиями
    public bool isGenerateDecorationBehindLocations; // не генерировать декорации за препятсвиями
}

public class Generator : MonoBehaviour
{
    public Location[] Locations;
    public int LengthRoad; // Длинна дороги
    [Space]
    public Barrier[] Barriers;
    [Range(.0f, 1f)]
    public float BarrierPercentage = 0.1f; // Процент заполнения барьерами (число от длины дороги 200 * 10% = 20 препятствий)
    [Space]
    public ChunkDicoretionGenerator[] Chunks;
    public Transform Finish;
    public GameObject EmptyDecoration; // Болванка для генерации декораций

    int[] Road; // id локаций
    Transform[] RoadTransform; // Ссылки на позиции локаций
    Transform[] BarrierTransform; // Ссылки на позиции препятствий
    int NumberBarraiers; // Количество генерируемых препятствий
    int SuitableLocations = 0; // Количество пригодных локаций для спавна препятствий
    string AllIdBarriers; // Id всех препятствий
    int LastIdBarrier = -1; // Предыдущее сгенерированное препятсиве
    


    void Start()
    {
        Generations();
        Finish.position = new Vector3(LengthRoad - 50, 0, 0);
    }

    /*Генерация элементов дороги локации*/
    void Generations()
    {
        GenerationRoad(); // генерация элементов дороги

        // подсчет количества генерируемых перпятсвий
        NumberBarraiers = Mathf.RoundToInt((SuitableLocations-2) * BarrierPercentage);

        GenerationBarriers(); // генерация перпятствий

        CreateChunks(); // Создание чанков для генерации декорацийs

        GenerationDecoration(); // Генерация декораций 
    }

    // генерация дороги
    void GenerationRoad()
    {
        var MaxLocations = Locations.Length; // Сколько всего видов локаций
        var NumberOfLocations = 0; // количество генерируемых локаций
        var IndexElement = 0; // индекс для генерации элемента дороги 
        // Необходимо количестов элементов дороги для всей длины локации
        NumberOfLocations = Mathf.CeilToInt(LengthRoad / Locations[0].Length) + 1;
        Road = new int[NumberOfLocations]; // количество id всех генерируемых дорог
        RoadTransform = new Transform[NumberOfLocations];

        /* проходится по всем элементам (кроме нулевого) и задает количество генераций локации,
         * в зависимости от общего процента (TotalPercentage)*/
        for (int i = 1; i < MaxLocations; i++)
        {
            Locations[i].Number = Mathf.FloorToInt(NumberOfLocations * Locations[i].TotalPercentage);
            for (int k = 0; k < Locations[i].Number; k++)
            {
                // учитывается шанс генерации элемента дороги
                if (Random.Range(0.0f, 1.0f) <= Locations[i].Chance)
                {
                    IndexSelectionCounter = 5; //счетсик выбора индекса, для избежания бесконечной рекурсии
                    // Выбор случайного свободного индекса (свободный, то есть выбрана smooth road по умолчанию)
                    IndexElement = SelectionIndexGenerationRoadElement();
                    if (IndexElement != 0)
                    {
                        Road[IndexElement] = i;
                        // резервирования соседних элементов, чтобы не было две рассщелини рядом друг с другом
                        Road[IndexElement - 1] = Road[IndexElement + 1] = -1;
                    }
                }
            }
        }

        Vector3 LocationPosition = new Vector3(-9.0f, .0f, .0f);
        GameObject newLocation;
        // Зачистка зарезервированных элементов (замена с -1 на 0)
        for (int i = 0; i < Road.Length; i++)
        {
            if (Road[i] == -1 || Road[i] == 0)
            {
                Road[i] = 0;
                SuitableLocations++;
            }
            /*Создание префабов в соответствии с id в массиве Road*/
            newLocation = Instantiate(Locations[Road[i]].Prefab, LocationPosition, Quaternion.identity);
            LocationPosition += new Vector3(Locations[Road[i]].Length, 0, 0);
            RoadTransform[i] = newLocation.transform;
            if (Road[i] > 0)
            {
                newLocation.name = Locations[Road[i]].RemoveDistance + "";
            }
        }
    }

    /*Функция возвращает номер индекса, в который присваивается id элемента дороги*/
    int IndexSelectionCounter = 5; // счетсик выбора индекса, от переполненияs
    int SelectionIndexGenerationRoadElement()
    {
        IndexSelectionCounter--;
        var rnd = Random.Range(2, Road.Length - 2); // Выбирается случайное число, кроме крайних элементов массива

        if (IndexSelectionCounter > 0)
        {
            // если у этого индекса массива дороги уже присвоен какой-то другой элемент дороги (то есть не smooth road), то
            if (Road[rnd] != 0 || Road[rnd - 1] != 0 || Road[rnd + 1] != 0)
            {
                // запускаем функцию выбора случайного индекса для элемента дороги ещё раз
                rnd = SelectionIndexGenerationRoadElement();
            }
        }
        else
        {
            rnd = 0;
            Debug.LogWarning("StackOverflow");
        }
        return rnd;

    }

    // генерация препятсивй 
    void GenerationBarriers()
    {
        BarrierTransform = new Transform[NumberBarraiers]; // Создание массива для ссылок на позиции препятствий

        // Записывается в строку все id препятствий
        AllIdBarriers = "";
        for (int k = 0; k < Barriers.Length; k++)
        {
            AllIdBarriers += k;
        }

        // исключение некотрых дорог из спавна препятствий
        for (int i = 0; i < Mathf.RoundToInt((SuitableLocations - 2) * (1f - BarrierPercentage)); i++)
        {
            var rnd = Random.Range(1, Road.Length - 1);
            if (Road[rnd] == 0)
            {
                Road[rnd] = -2;
            }
            else
            {
                i--;
            }
        }

        // генерация препятствий
        GameObject newBarrier;
        for (int i = 1; i < Road.Length-1; i++)
        {
            if (Road[i] == 0)
            {
                // Выбирается id барьера из строки всех id исключая id предыдущего препятствия 
                var NumBarrier = SelectBarrier(AllIdBarriers.Replace(LastIdBarrier + "", ""));

                newBarrier = Instantiate(Barriers[NumBarrier].Prefab,
                    RoadTransform[i].position + new Vector3(Locations[0].Length / 2
                    + Random.Range(Barriers[NumBarrier].RangeGenerationDistance.x,
                    Barriers[NumBarrier].RangeGenerationDistance.y), .0f, .0f), Quaternion.identity);
                // Запись позиции барьера в массив
                BarrierTransform[NumberBarraiers-1] = newBarrier.transform;
                newBarrier.name = Barriers[NumBarrier].RemovealRadiusDecoretion + "";
                NumberBarraiers--;
            }
        }
    }

    // Выбор какое препятсиве генерировать
    int SelectBarrier(string ids)
    {
        var rnd = 0; // случайный id перпятствия
        var index = 0; // Случайны номер символа из строки

        // цикл выполняется до тех пор, пока в строке не останется один id препятсвия
        do
        {
            index = Random.Range(0, ids.Length);
            rnd = int.Parse(ids.Substring(index, 1));
            // Елсли шанс генерации выбранного препятствия больше
            if (Random.Range(0.0f, 1.0f) > Barriers[rnd].Chance)
            {
                // то id этого препятсивя исключается из строки
                ids = ids.Remove(index, 1);
            }
        }
        while (ids.Length != 1);
        LastIdBarrier = int.Parse(ids);
        return LastIdBarrier;
    }

    void CreateChunks()
    {
        // Цикл проходится по всем чанкам (Дальние декорации, ближние декорации, декорации под землей)
        for (int i = 0; i < Chunks.Length; i++)
        {
            // Создать чанк по его номеру
            GenerationChunk(i);
        }
    }

    void GenerationChunk(int index)
    {
        // Задаётся количестов ячеек в одном чанке
        Chunks[index].CellChunk = new Transform[Chunks[index].Cells.x * Chunks[index].Cells.y];

        // Рассчйт индекс одной ячейки, число на которое умножается номер ячейки для получения её позиции
        Vector2 factor = new Vector2(Chunks[index].Chunk.localScale.x / Chunks[index].Cells.x,
                                     Chunks[index].Chunk.localScale.y / Chunks[index].Cells.y);
        
        // Создание точек генерации в одном чанке
        GameObject newCell; // Новая генерируемая клетка чанка
        Vector3 newPos; // Новая позиция генерирумой клетки в чанке
        for (int r = 0; r < Chunks[index].Cells.x; r++)
        {
            for (int c = 0; c < Chunks[index].Cells.y; c++)
            {
                // Рассчёт позиции клетки чанка, индекс умножается на номер клатки в строки и стоблце
                newPos = new Vector3(Chunks[index].Chunk.position.x + r * factor.x,
                                    Chunks[index].Chunk.position.y + c * factor.y, 0);

                newCell = new GameObject();
                newCell.transform.position = newPos;
                newCell.transform.parent = Chunks[index].Chunk.transform;
                newCell.name = r + "" + c;
            }
        }

        /* Присвоение клеток чанка в массив*/
        int indexChildCell = -1; // Индекс для поочередного присвоения в массив
                                 // Поиск всех дочерних объектов чанка и присвоение их в массив этого чанка
        foreach (Transform cell in Chunks[index].Chunk.GetComponentsInChildren<Transform>())
        {
            // Первый найденный объект это и есть сам чанк, его нужно пропустить
            if (indexChildCell == -1)
            {

            }
            else
            {
                Chunks[index].CellChunk[indexChildCell] = cell;
            }
            indexChildCell++;
        }
    }

    //Создание строки с номерами всех допустимых клеток для генерации
    void CreatePossibleCells(int index) {
        for (int row = 0; row < Chunks[index].Cells.x; row++)
        {
            for (int col = 0; col < Chunks[index].Cells.y; col++)
            {
                Chunks[index].AvailableCells += row + "" + col + ":";
            }
        }
        Chunks[index].PossibleCells = Chunks[index].AvailableCells;
    }

    // Генерация декораций 
    void GenerationDecoration()
    {
        // Цикл проходится по всем чанкам (Дальние декорации, ближние декорации, декорации под землей)
        for (int i = 0; i < Chunks.Length; i++)
        {
            for (int dist = 0; dist < Mathf.CeilToInt(LengthRoad / Chunks[i].Cells.x); dist++)
            {
                //Создание строки с номерами всех допустимых клеток для генерации
                CreatePossibleCells(i);
                // Цикл проходит по всем декорациям в одном чанке
                for (int d = 0; d < Chunks[i].Decorations.Length; d++)
                {
                    /* Генерация всех видов декораций для одного чанка
                     * количество каждого вида декорации выбирается случайно в поле Number от min до max*/
                    for (int t = 0; t < Random.Range(Chunks[i].Decorations[d].Number.x, Chunks[i].Decorations[d].Number.y); t++)
                    {
                        // Шанс генеарции декорации
                        if (Random.Range(.0f, 1f) <= Chunks[i].Decorations[d].Chance)
                        {
                            // Выбор ячейки для генерации декорации и диактивация окружающих
                            int indexCell = SelectCell(i);
                            if (indexCell != -1)
                            {
                                // Генерация спрайта декорации
                                GenerationSpriteDecoration(i, d, indexCell);
                            }
                        }
                    }
                }
                Chunks[i].Chunk.position += new Vector3(Chunks[i].Cells.x, 0, 0);
            }
        }
    }

    // Выбор ячейки для генерации декорации и диактивация окружающих
    int SelectCell(int index)
    {
        string[] AllCells = Chunks[index].PossibleCells.Split(char.Parse(":"));
        // получаю имя случайной клетки содержащиеся в строке доступных клеток
        var rndNameIndexAllCells = AllCells[Random.Range(0, AllCells.Length)];
        // полчуаю индекс в массиве выбранной клетки
        var rnd = 0;
        for (int c = 0; c < Chunks[index].CellChunk.Length; c++)
        {
            // если имя совпадает с зарезервированной клеткой, то отключаем ее и выходим из цикла
            if (Chunks[index].CellChunk[c].name == rndNameIndexAllCells)
            {
                rnd = c;
                c = Chunks[index].CellChunk.Length;
            }
        }

        // Получение имени клетки, в которой будет сгенерирована декорация
        string nameCell = Chunks[index].CellChunk[rnd].name;
        // Разбиение имени клетик на номер строки и номер столбца этой лкетки
        int row = int.Parse(nameCell.Substring(0, 1)) - 1;
        int cel = int.Parse(nameCell.Substring(1, 1)) - 1;

        //Исключение и отключение зарезервированных клеток из доступных клеток
        for (int i = row; i < row + 3; i++)
        {
            for(int k = cel; k < cel + 3; k++)
            {
                Chunks[index].PossibleCells = Chunks[index].PossibleCells.Replace(i + "" +  k + ":", "");

                // отключение зарезервированных клеток
                for(int c = 0; c < Chunks[index].CellChunk.Length; c++)
                {
                    // если имя совпадает с зарезервированной клеткой, то отключаем ее и выходим из цикла
                    if(Chunks[index].CellChunk[c].name == i + "" + k)
                    {
                        c = Chunks[index].CellChunk.Length;
                    }
                }
            }
        }
        return rnd;
    }

    // Генерация спрайта декорации
    GameObject newSpriteDecoration;
    int rndSprite; // Случайный спрайт
    int layer;
    bool isPossiblegenerate = true;
    float RemoveRadius;
    void GenerationSpriteDecoration(int i, int d, int indexCell)
    {
        isPossiblegenerate = true;
        // не генерировать декорации за препятствиями?
        if (Chunks[i].isGenerateDecorationBehindBarriers)
        {
            // Пролистывается массив со всем препятствиями и проверяется его позиция плюс радиус не генерации декораций
            for (int p = 0; p < BarrierTransform.Length; p++)
            {
                // если декорация попадает в радиус удаления данного препятсвия, то декорация не генерируется
                RemoveRadius = float.Parse(BarrierTransform[p].name);
                if (Chunks[i].CellChunk[indexCell].position.x > BarrierTransform[p].position.x - RemoveRadius
                    && Chunks[i].CellChunk[indexCell].position.x < BarrierTransform[p].position.x + RemoveRadius)
                {
                    isPossiblegenerate = false;
                }
            }
        }
        if (Chunks[i].isGenerateDecorationBehindLocations)
        {
            // Декорации  не генерируются на оврагах
            for (int p = 0; p < RoadTransform.Length; p++)
            {
                // Если id дороги не "ровная"
                if (Road[p] > 0)
                {
                    // то высчитывается радиус диактивации декораций
                    RemoveRadius = float.Parse(RoadTransform[p].gameObject.name);
                    // и считается радиус от начала элемента дороги + пол его длины высчитывается радиус в обе стороны
                    if (Chunks[i].CellChunk[indexCell].position.x > RoadTransform[p].position.x + Locations[Road[p]].Length / 2 - RemoveRadius
                        && Chunks[i].CellChunk[indexCell].position.x < RoadTransform[p].position.x + Locations[Road[p]].Length / 2 + RemoveRadius)
                    {
                        isPossiblegenerate = false;
                    }
                }
            }
        }
        if (isPossiblegenerate)
        {
            newSpriteDecoration = Instantiate(EmptyDecoration, Chunks[i].CellChunk[indexCell].position,
                                            Quaternion.Euler(0, 0, Random.Range(Chunks[i].Decorations[d].RandomRotation.x,
                                            Chunks[i].Decorations[d].RandomRotation.y)));
            //Выбор случайного спрайта из массива Object
            rndSprite = Random.Range(0, Chunks[i].Decorations[d].Object.Length);
            newSpriteDecoration.GetComponent<SpriteRenderer>().sprite = Chunks[i].Decorations[d].Object[rndSprite];
            newSpriteDecoration.name = indexCell + "";
            layer = int.Parse(Chunks[i].CellChunk[indexCell].name.Substring(1, 1));
            newSpriteDecoration.GetComponent<SpriteRenderer>().sortingOrder = Chunks[i].CellChunk.Length - layer;
            newSpriteDecoration.GetComponent<SpriteRenderer>().sortingLayerName = Chunks[i].Name;

            // Если облако
            if (!Chunks[i].isGenerateDecorationBehindLocations)
            {
                newSpriteDecoration.GetComponent<SpriteRenderer>().sortingOrder = -4;
                newSpriteDecoration.tag = "Cloud";
            }
        }
    }
}
