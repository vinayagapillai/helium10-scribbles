using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private GameObject _getCSvPanel;
    
    [SerializeField] private KeywordsHolder _keywordHolderPrefab;
    [SerializeField] private Transform _nonUsedKeywordHolderParent;
    [SerializeField] private Transform _usedKeywordHolderParent;

    private List<KeywordsHolder> _allKeywordsHolders;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        EnableGetCSVPanel(true);
    }

    public void EnableGetCSVPanel(bool active)
    {
        _getCSvPanel.SetActive(active);
    }

    public void UpdateColors()
    {
    }

    public void PopulateKeyordList(List<DataItem> dataItems)
    {
        _allKeywordsHolders = new List<KeywordsHolder>();
        foreach (DataItem item in dataItems)
        {
            KeywordsHolder holder = Instantiate(_keywordHolderPrefab, _nonUsedKeywordHolderParent);
            holder.KeywordNameUI.text = item.Keyword;
            holder.VolumeUI.text = "" + item.Volume;
            holder.BGImage.color = item.KeywordColor;
            _allKeywordsHolders.Add(holder);
        }
        UpdateKeywordList(dataItems);
    }

    public void UpdateKeywordList(List<DataItem> dataItems)
    {
        for (int i = 0; i < dataItems.Count; i++)
        {
            _allKeywordsHolders[i].NOOfUsageUI.text = "" + dataItems[i].NumberOfTimesUsed;
            if (dataItems[i].NumberOfTimesUsed > 0)
            {
                _allKeywordsHolders[i].transform.SetParent(_usedKeywordHolderParent);
            }
            else
            {
                _allKeywordsHolders[i].transform.SetParent(_nonUsedKeywordHolderParent);
            }
        }

        KeywordsHolder[] holder = _nonUsedKeywordHolderParent.GetComponentsInChildren<KeywordsHolder>();
        holder = holder.OrderByDescending(volume => int.Parse(volume.VolumeUI.text)).ToArray();

        for (int i = 0; i < holder.Length; i++)
        {
            holder[i].transform.SetSiblingIndex(i);   
        }
        
        holder = _usedKeywordHolderParent.GetComponentsInChildren<KeywordsHolder>();
        holder = holder.OrderByDescending(volume => int.Parse(volume.VolumeUI.text)).ToArray();

        for (int i = 0; i < holder.Length; i++)
        {
            holder[i].transform.SetSiblingIndex(i);   
        }
    }
    
}