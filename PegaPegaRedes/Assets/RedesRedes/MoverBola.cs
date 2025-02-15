using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MoverBola : NetworkBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 dir;
    public float v;
    
    [SerializeField] private Color[] cores; // [0] = Pegador (vermelho), [1] = Normal (azul)

    // Variável de rede para definir quem é o pegador
    private NetworkVariable<bool> isPegador = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnNetworkSpawn()
    {
        transform.position = new Vector2(Random.Range(-3,3),Random.Range(-3,3));
        sr = GetComponent<SpriteRenderer>();

        // O primeiro jogador a entrar (Host) será o pegador inicial
        if (IsServer && NetworkManager.Singleton.ConnectedClients.Count == 1)
        {
            isPegador.Value = true;
        }

        // Listener para atualizar a cor sempre que o estado do pegador mudar
        isPegador.OnValueChanged += (oldValue, newValue) => AtualizarCor();
        
        // Aplicar cor inicial
        AtualizarCor();
    }

    void Update()
    {
        if (IsOwner)
        {
            MoverBolita();
        }
    }

    void MoverBolita()
    {
        dir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
        rb.velocity = dir * v;
    }

    void AtualizarCor()
    {
        sr.color = isPegador.Value ? cores[0] : cores[1]; // Vermelho se pegador, azul se não
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!IsOwner || !IsServer) return; // Apenas o servidor pode decidir quem é o pegador

        if (col.gameObject.CompareTag("p"))
        {
            var outroJogador = col.gameObject.GetComponent<MoverBola>();
            if (outroJogador != null && outroJogador.isPegador.Value != isPegador.Value)
            {
                // Chama um ServerRpc para alternar os papéis dos jogadores
                TrocarPegadorServerRpc(outroJogador.OwnerClientId);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void TrocarPegadorServerRpc(ulong novoPegadorId)
    {
        // Alterna os estados dos jogadores
        isPegador.Value = !isPegador.Value;
        NetworkManager.Singleton.ConnectedClients[novoPegadorId].PlayerObject.GetComponent<MoverBola>().isPegador.Value = !isPegador.Value;
    }
}
