using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class InventoryManager : MonoBehaviour
{
    private Dictionary<string, int> ingredientCounts = new Dictionary<string, int>();
    public List<Ingred> ingreList = new List<Ingred>(); // CSV���� �ҷ��� ��� ����Ʈ
    private GameData gameData; // DataManager���� �ҷ��� ������
    private string csvFileName = "ingredient.csv"; // CSV ���ϸ�

    [System.Serializable]
    public class Ingred
    {
        public int index;
        public string name;
        public int type;
        public int price;

        public Ingred(int index, string name, int type, int price)
        {
            this.index = index;
            this.name = name;
            this.type = type;
            this.price = price;
        }
    }

    void Start()
    {
        // CSV���� ��� ���� �ҷ�����
        LoadIngredientsFromCSV();

        // DataManager���� ����� ��� ���� �ҷ�����
        LoadIngredientsFromGameData();
    }

    // GameDataManager���� ��� ���� �ҷ�����
    private void LoadIngredientsFromGameData()
    {
        gameData = DataManager.Instance?.LoadGameData();

        if (gameData == null)
        {
            Debug.LogError("LoadIngredientsFromGameData: GameData�� null�Դϴ�! DataManager���� �����͸� ������ �� �����ϴ�.");
            return;
        }

        if (gameData.ingredientNum == null)
        {
            Debug.LogWarning("LoadIngredientsFromGameData: ingredientNum�� null�̹Ƿ� �� ����Ʈ�� �ʱ�ȭ�մϴ�.");
            gameData.ingredientNum = new List<int>(new int[ingreList.Count]); // �� ����Ʈ �ʱ�ȭ
        }

        // CSV���� �ҷ��� ��� ����Ʈ�� ��Ī�Ͽ� ���� ����
        for (int i = 0; i < ingreList.Count; i++)
        {
            string ingredientName = ingreList[i].name;
            int count = (i < gameData.ingredientNum.Count) ? gameData.ingredientNum[i] : 0;
            ingredientCounts[ingredientName] = count;
        }

        Debug.Log("��� �����͸� ���������� �ҷ��Խ��ϴ�.");
        PrintCurrentInventory();
    }

    // ��� ���� ����
    private void SaveIngredients()
    {
        if (gameData == null)
        {
            Debug.LogError("SaveIngredients: GameData�� null�̹Ƿ� ������ �� �����ϴ�.");
            return;
        }

        // ingredientCounts ��ųʸ� ���� ingredientNum ����Ʈ�� ��ȯ
        List<int> updatedIngredientNum = new List<int>();

        foreach (var ingredient in ingreList)
        {
            if (ingredientCounts.ContainsKey(ingredient.name))
            {
                updatedIngredientNum.Add(ingredientCounts[ingredient.name]);
            }
            else
            {
                updatedIngredientNum.Add(0);
            }
        }

        // ������Ʈ�� ������ ����
        gameData.ingredientNum = updatedIngredientNum;
        DataManager.Instance.SaveGameData();
    }

    // CSV���� ��� ��� �ҷ�����
    private void LoadIngredientsFromCSV()
    {
        try
        {
            TextAsset csvFile = Resources.Load<TextAsset>("ingredient"); // `Resources/ingredient.csv`���� �ε�
            if (csvFile == null)
            {
                Debug.LogError("CSV ������ ã�� �� �����ϴ�: ingredient.csv");
                return;
            }

            string[] lines = csvFile.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

            for (int i = 1; i < lines.Length; i++) // ù ��° ��(���) ����
            {
                string[] fields = lines[i].Split(',');
                if (fields.Length < 4) continue;

                int index = int.Parse(fields[0].Trim());
                string name = fields[1].Trim();
                int type = int.Parse(fields[2].Trim());
                int price = int.Parse(fields[3].Trim());

                ingreList.Add(new Ingred(index, name, type, price));
            }

            Debug.Log($"�� {ingreList.Count}���� ��Ḧ CSV���� �ҷ��Խ��ϴ�.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"CSV ���� �б� �� ���� �߻�: {ex.Message}");
        }
    }

    // ��� �߰�
    public void AddIngredient(string ingredient, int count = 1)
    {
        if (ingredientCounts.ContainsKey(ingredient))
        {
            ingredientCounts[ingredient] += count;
        }
        else
        {
            ingredientCounts[ingredient] = count;
        }

        SaveIngredients();
    }

    // ��� ���� ���� (���)
    public bool UseIngredient(string ingredient)
    {
        if (ingredientCounts.ContainsKey(ingredient) && ingredientCounts[ingredient] > 0)
        {
            ingredientCounts[ingredient]--;
            SaveIngredients();
            return true;
        }
        return false;
    }

    // ��� ���� ����
    public bool HasIngredient(string ingredient)
    {
        return ingredientCounts.ContainsKey(ingredient) && ingredientCounts[ingredient] > 0;
    }

    // ���� ������ ��� ��� ���
    private void PrintCurrentInventory()
    {
        Debug.Log("���� ������ ���:");
        foreach (var ingredient in ingredientCounts)
        {
            Debug.Log($"- {ingredient.Key}: {ingredient.Value}��");
        }
    }
}
