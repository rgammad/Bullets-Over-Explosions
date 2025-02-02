﻿using UnityEngine;
using System.Collections;

public class PlayerBulletAPI : MonoBehaviour {
    GameObject piercingLaserPrefab;
    GameObject bouncingLaserPrefab;
    GameObject explodingLaserPrefab;
    GameObject explosionSystemPrefab;
    GameObject explosionTriggerPrefab;

    void Awake()
    {
        piercingLaserPrefab = Resources.Load("Prefabs/PiercingLaser") as GameObject;
        bouncingLaserPrefab = Resources.Load("Prefabs/BouncingLaser") as GameObject;
        explodingLaserPrefab = Resources.Load("Prefabs/ExplodingLaser") as GameObject;
        explosionSystemPrefab = Resources.Load("Prefabs/ExplosionSystemBlue") as GameObject;
        explosionTriggerPrefab = Resources.Load("Prefabs/ExplosionTrigger") as GameObject;        
    }    

    public void PiercingShot(Vector3 mousePos)
    {
        GameObject laser = (GameObject)Instantiate(piercingLaserPrefab);
        LineRenderer laserRender = laser.GetComponent<LineRenderer>();

        Vector3 initialPos = this.transform.position;
        Vector3 targetPos = mousePos - initialPos;

        Vector3 laserEndPos = targetPos.normalized * 100;

        RaycastHit2D[] hitList = Physics2D.RaycastAll(initialPos, targetPos);
        foreach (RaycastHit2D hit in hitList)
        {
            if (hit.collider.CompareTag("Wall"))
            {
                laserEndPos = hit.point;
                break;
            }
            if (hit.collider.CompareTag("Enemy"))
            {
                hit.collider.tag = "Dead";
            }
            if (hit.collider.CompareTag("Object"))
            {
                hit.collider.tag = "Explode";
            }
        }

        laserRender.SetPosition(0, this.transform.position);
        laserRender.SetPosition(1, laserEndPos);
    }

    public void BouncingShot(Vector3 mousePos)
    {
        GameObject laser = (GameObject)Instantiate(bouncingLaserPrefab);
        LineRenderer laserRenderer = laser.GetComponent<LineRenderer>();
        
        Vector3 initialPos = this.transform.position;
        Vector3 targetPos = mousePos - initialPos;
        laserRenderer.SetPosition(0, initialPos);

        for (int i = 1; i <= 5; i++)
        {
            targetPos = targetPos.normalized * 100;
            Vector3 vertexPos = targetPos;
            initialPos += (targetPos).normalized * 0.1f;
            RaycastHit2D[] hitList = Physics2D.RaycastAll(initialPos, targetPos);            

            foreach (RaycastHit2D hit in hitList)
            {
                if (hit.collider.CompareTag("Wall"))
                {                    
                    vertexPos = hit.point;
                    initialPos = hit.point;
                    targetPos = Vector2.Reflect(targetPos, hit.normal);
                    break;
                }
                if (hit.collider.CompareTag("Enemy"))
                {
                    hit.collider.tag = "Dead";
                }
                if (hit.collider.CompareTag("Object"))
                {
                    hit.collider.tag = "Explode";
                }
            }

            laserRenderer.SetPosition(i, vertexPos);
        }
    }

    public void ExplodingShot(Vector3 mousePos)
    {
        GameObject laser = (GameObject)Instantiate(explodingLaserPrefab);
        LineRenderer laserRender = laser.GetComponent<LineRenderer>();

        Vector3 initialPos = this.transform.position;
        Vector3 targetPos = mousePos - initialPos;

        Vector3 laserEndPos = targetPos.normalized * 100;
        
        RaycastHit2D hit = Physics2D.Raycast(initialPos, targetPos);
        if (hit != null)
        {
            laserEndPos = hit.point;

            Instantiate(explosionSystemPrefab, hit.point, Quaternion.identity);
            Instantiate(explosionTriggerPrefab, hit.point, Quaternion.identity);

            if (hit.collider.CompareTag("Enemy"))
            {
                hit.collider.tag = "Dead";
            }
            if (hit.collider.CompareTag("Object"))
            {
                hit.collider.tag = "Explode";
            }        
        }

        laserRender.SetPosition(0, this.transform.position);
        laserRender.SetPosition(1, laserEndPos);
    }
}
