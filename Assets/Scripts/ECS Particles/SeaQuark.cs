using UnityEngine;
using Unity.Mathematics;

public class SeaQuark : MonoBehaviour {
    public float HP;
    public float virtuality;
    public float x;
    public float cageRadius;
    public QuarkColor color;

    private GameObject valenceQuarkDown;
    private GameObject valenceQuarkUpRed;
    private GameObject valenceQuarkUpBlue;

    private Vector3 dirAvg;

    private Vector3 RandomUnitVector() {
        return new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
    }

    private float3 calculateValenceQuarkForce(GameObject quark) {
        // valenceQuarkDown.transform.position - transform.position
        // return vectorToQuark / math.max(0.1f, math.lengthsq(vectorToQuark));
        if (quark == null)
            return 0;

        float3 vectorToQuark = (float3) (quark.transform.position - this.transform.position);
        float3 f = vectorToQuark / math.max(0.1f, math.lengthsq(vectorToQuark));

        if(quark.GetComponent<Quark>().quarkColor == this.color) {
            return f;
        } else {
            return -1 * f;
        }
    }

    private Vector3 randomnessFromVirtuality() {
        Vector3 r = Vector3.zero;
        if(virtuality > 0.1f) {
            Vector3 shake = RandomUnitVector() * virtuality * 10f;
            r += shake;

            if(UnityEngine.Random.Range(0, 900 - (int)(virtuality * 800)) == 1) { // range(0,~100) good for maximum jumpiness
                Vector3 jump = RandomUnitVector() * 150f;
                r += jump;
                this.dirAvg = r; // reset position history - also makes rotation look random
            }
        }
        return r;
    }

    public void Start() {
        HP = 1f;
        color = QuarkColor.Red;
        valenceQuarkDown = GameObject.Find("Quark3_Down");
        valenceQuarkUpRed = GameObject.Find("Quark1_Up_Red");
        valenceQuarkUpBlue = GameObject.Find("Quark2_Up_Blue");

        dirAvg = RandomUnitVector() * 5;
        transform.rotation = Quaternion.LookRotation(dirAvg);
    }

    public void Update() {
        QuarkPair[] qs = GetComponentsInChildren<QuarkPair>();
        //Vector3 center = (qs[1].transform.position + qs[0].transform.position) / 2;

        //GetComponentInChildren<SkinnedMeshRenderer>().transform.parent.position = center;

        //qs[1].transform.position = center + ((qs[1].transform.position - center).normalized * (transform.localScale.z / 2f));
        //qs[0].transform.position = center + ((qs[0].transform.position - center).normalized * (transform.localScale.z / 2f));

        //GetComponentInChildren<SkinnedMeshRenderer>().transform.parent.LookAt(qs[1].transform.position, GetComponentInChildren<SkinnedMeshRenderer>().transform.up);

        float3 valenceQuarkForce = calculateValenceQuarkForce(valenceQuarkDown) +
            calculateValenceQuarkForce(valenceQuarkUpRed) +
            calculateValenceQuarkForce(valenceQuarkUpBlue);

        Vector3 dir = valenceQuarkForce * 2.3f;
        dirAvg = dirAvg * 0.9f + dir * 0.1f;

        // Randomness from virtuality
        dir += randomnessFromVirtuality();
        dir *= Time.deltaTime;

        if(math.lengthsq(transform.position + dir) > math.pow(cageRadius, 2)) {
            dir = Vector3.zero;
        }

        //transform.position += dir;
        //transform.rotation = Quaternion.LookRotation(dirAvg, Vector3.up);

        if(HP <= 0) {
            FindObjectOfType<SeaQuarkSpawner>().Energy++;
            Destroy(this.gameObject);
        } else {
            HP -= virtuality * 1.5f * Time.deltaTime; // TODO determine lifetime scaling with virtuality
        }
    }
}