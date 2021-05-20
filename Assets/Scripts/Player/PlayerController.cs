using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float maxHealth = 100f;
    private float _playerHealth;
    public TextMeshProUGUI healthBar;

    private void Awake()
    {
        PlayerHealth = maxHealth;
    }

    public float PlayerHealth
    {
        get
        {
            return _playerHealth;
        }
        set
        {
            _playerHealth = value;
            healthBar.text = "HEALTH: " + value;
            if (_playerHealth <= 0) OnPlayerDead();

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReceiveDamage(float damage)
    {
        PlayerHealth -= damage;

    }

    private void OnPlayerDead()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}