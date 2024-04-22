using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_run : MonoBehaviour
{
    [SerializeField] private GameObject pl;
    [SerializeField] private Transform from;
    [SerializeField] private Transform to;
    private List<Transform> players = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 300; i++)
        {
            GameObject g = Instantiate(pl);
            g.transform.position = from.position + (Random.insideUnitSphere * 5);
            g.SetActive(true);
            players.Add(g.transform);
        }

        StartCoroutine(play());
    }
    private IEnumerator play()
    {
        while (true)
        {
            for (int i = 0; i < players.Count; i++)
            {
                players[i].DOMove(new Vector3(Random.Range(to.position.x+10, to.position.x - 10), 0, Random.Range(to.position.z + 5, to.position.z - 5)), 5).SetEase(Ease.Linear);
            }

            yield return new WaitForSeconds(5);

            for (int i = 0; i < players.Count; i++)
            {
                players[i].DOMove(new Vector3(Random.Range(from.position.x + 10, from.position.x - 10), 0, Random.Range(from.position.z + 5, from.position.z - 5)), 5).SetEase(Ease.Linear);
            }

            yield return new WaitForSeconds(5);
        }
    }
    
}
