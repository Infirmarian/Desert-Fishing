﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {


	// Update is called once per frame
	void Update () {
        if (Input.GetButton("Fire1"))
            SceneManager.LoadScene("Main");
	}
}
