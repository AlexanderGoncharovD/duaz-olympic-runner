using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*Отвечает за перключение языков на всех текстовых элементах;
 * вешается на любой неудалящийся объект на сцене
 * Добавление нового языка: 
 * 1) В enum добавить общеприянтое сокращение языка
 * 2) В структуры добавить текстовую переменную с переводом на добавляемый язык
*/

[System.Serializable]
public struct TextsElemetns // Структура для текстовых элементов реализованных через UI text
{
#if UNITY_EDITOR
    public string Name; // Имя группы в редакторе
#endif
    public Text TextElement; // Ссылка на объект содержащий текст
    public string En; // Как текстовый элемент пишется на английском языке
    public string Ru; // Как текстовый элемент пишется на русском языке
}

[System.Serializable]
public struct TextsMeshElemetns // Структура для текстовых элементов реализованных через 3D текст
{
#if UNITY_EDITOR
    public string Name; // Имя группы в редакторе
#endif
    public TextMesh TextElement; // Ссылка на объект содержащий текст
    public string En; // Как текстовый элемент пишется на английском языке
    public string Ru; // Как текстовый элемент пишется на русском языке
}

public enum AllLanguages // Перечисление всех доступных языков в игре
{
    EN,
    RU
}

public class Languages : MonoBehaviour
{
    public TextsElemetns[] Texts; // Массив UI text
    public TextsMeshElemetns[] TextsMesh; // Массив 3D text
    public AllLanguages Language; // Перечисленеи языков
    
    [ContextMenu("UpdateLanguage")] // Запуск функции через редактор
	void UpdateLanguage()
    {
		switch (Language)
        {
            case AllLanguages.RU:
                // Присвоение всем UI текстовым элементам русский язык
                for(int i = 0; i < Texts.Length; i++)
                {
                    Texts[i].TextElement.text = Texts[i].Ru;
                }

                // Присвоение всем 3D текстовым элементам русский язык
                for (int i = 0; i < TextsMesh.Length; i++)
                {
                    TextsMesh[i].TextElement.text = TextsMesh[i].Ru;
                }
                break;
            case AllLanguages.EN:
                // Присвоение всем UI текстовым элементам английский язык
                for (int i = 0; i < Texts.Length; i++)
                {
                    Texts[i].TextElement.text = Texts[i].En;
                }

                // Присвоение всем 3D текстовым элементам английский язык
                for (int i = 0; i < TextsMesh.Length; i++)
                {
                    TextsMesh[i].TextElement.text = TextsMesh[i].En;
                }
                break;
        }
	}
}
