using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class FossilCleaning : MonoBehaviour
{
    public static FossilCleaning Instance; // Singleton para acesso fï¿½cil

    [Header("UI Elements")]
    [SerializeField] private GameObject cleaningUI; // Canvas ou Painel principal da limpeza (ative/desative)
    [SerializeField] private Button[] toolButtons; // Array de 4 botï¿½es: [0] Pincel, [1] Espï¿½tula, [2] Martelo, [3] Estaca
    [SerializeField] private Transform fossilDisplayParent; // Parent para posicionar o fï¿½ssil aumentado na UI
    [SerializeField] private Slider toolProgressBar; // Barra de progresso para ferramenta atual (% cobertura)

    [Header("Configuraï¿½ï¿½es de Limpeza")]
    [SerializeField] private int numTools = 4; // Nï¿½mero de ferramentas

    [Header("Cobertura de ï¿½rea")]
    [SerializeField] private int gridSize = 3; // Resoluï¿½ï¿½o da grid (ex.: 10x10 = 100 pontos no fï¿½ssil)
    [SerializeField] private float coverageThreshold = 0.8f; // % mï¿½nimo coberto para completar uma ferramenta (0.8 = 80%)
    [SerializeField] private float brushRadius = 1.5f; // Raio de "pincelada" na grid (marca mï¿½ltiplas cï¿½lulas por movimento)

    [Header("Feedback Visual")]
    [SerializeField] private GameObject brushIndicator; // Sprite/cï¿½rculo que segue o mouse durante LMB (crie na UI)
    [SerializeField] private Transform dirtOverlay; // Parent para grid de sujeira (no prefab do fï¿½ssil; sub-sprites DirtCell_X_Y)

    [Header("Cï¿½mera de Isolamento")]
    [SerializeField] private Camera cleaningCamera; // Cï¿½mera secundï¿½ria para focar no fï¿½ssil e UI (crie uma na cena)
    [SerializeField] private LayerMask cleaningLayerMask = -1; // LayerMask para renderizar sï¿½ UI + fï¿½ssil (configure no Inspector)
    [SerializeField] private Vector3 cleaningCameraOffset = new Vector3(0, 0, -10); // Offset da cï¿½mera para o fï¿½ssil (ajuste para zoom/close-up)

    private bool[] toolsCompleted; // Rastreia conclusï¿½o por ferramenta (true se coberta 80%+)
    private bool[,] currentCoverageGrid; // Grid de cobertura para a ferramenta ATUAL (reseta por ferramenta)
    private int currentToolIndex = -1; // Ferramenta selecionada (-1 = nenhuma)
    private GameObject currentFossil; // Referï¿½ncia ao fï¿½ssil sendo limpo
    private Bounds fossilBounds; // Bounds do fï¿½ssil para mapear grid
    private InteractionFase3 interaction; // Referï¿½ncia ao script do jogador
    private Camera mainCamera; // Cï¿½mera principal (mundo/jogador)
    public GameObject[] dirtCells; // Array para os sub-sprites de sujeira (gridSize x gridSize)
    [SerializeField] private HudFase3 placar;
    [SerializeField] private InteractionFase3 player;
    float dirtScaleFactor = 0.4f;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
#if !UNITY_EDITOR
            DontDestroyOnLoad(gameObject); // Sï¿½ em builds, nï¿½o no Editor
#endif
            toolsCompleted = new bool[numTools];
            mainCamera = Camera.main;
            if (cleaningCamera == null)
            {
                Debug.LogWarning("FossilCleaning: Crie uma Camera secundï¿½ria e arraste para 'Cleaning Camera' no Inspector!");
            }
            cleaningUI.SetActive(false); // Inicialmente inativo
            SetupToolButtons(); // Configura listeners nos botï¿½es
            if (brushIndicator != null) brushIndicator.SetActive(false); // Indicator inativo inicialmente
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        interaction = FindObjectOfType<InteractionFase3>();
        if (interaction == null)
            Debug.LogError("FossilCleaning: InteractionFase3 não encontrado na cena!");

        placar = FindObjectOfType<HudFase3>();
        if (placar == null)
            Debug.LogError("FossilCleaning: HudFase3 não encontrado na cena!");

        player = FindObjectOfType<InteractionFase3>();

    }

    void Update()
    {
        if (currentFossil == null || !cleaningUI.activeInHierarchy || currentToolIndex < 0) return;

        Camera activeCam = cleaningCamera != null ? cleaningCamera : mainCamera; // Usa cleaningCamera se disponï¿½vel

        // Move o brush indicator para posiï¿½ï¿½o do mouse se LMB pressionado
        if (Mouse.current.leftButton.isPressed && brushIndicator != null)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 worldPos = activeCam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, activeCam.nearClipPlane));
            brushIndicator.transform.position = worldPos;
            brushIndicator.SetActive(true); // Mostra o indicator durante pincelada
        }
        else if (brushIndicator != null)
        {
            brushIndicator.SetActive(false); // Esconde quando nï¿½o pressionando
        }

        // Uso da ferramenta: Raycast no fï¿½ssil inteiro (segurar clique esquerdo)
        if (Mouse.current.leftButton.isPressed)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 worldPos = activeCam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, activeCam.nearClipPlane));
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == currentFossil)
            {
                // Marca cobertura na grid baseada na posiï¿½ï¿½o do hit
                MarkCoverage(hit.point);
                CheckToolCompletion(); // Verifica se ferramenta atual estï¿½ completa
            }
        }

        // Verifica conclusï¿½o geral a cada frame
        CheckCompletion();
    }

    // Mï¿½todo pï¿½blico: Inicia a limpeza (chamado pelo InteractionFase3 apï¿½s escavaï¿½ï¿½o)
    public void StartCleaning(GameObject fossil)
    {
        if (interaction == null || interaction.currentState != PlayerState.Cleaning) return;

        currentFossil = fossil;
        cleaningUI.SetActive(true); // Ativa UI (botï¿½es visï¿½veis)

        // Pausa movimento do jogador (reforï¿½o)
        if (interaction != null)
        {
            interaction.PausePlayerMovement(true); // Pausa inputs e movimento
        }

        // Aumenta e posiciona o fï¿½ssil na UI
        if (fossilDisplayParent != null)
        {
            fossil.transform.SetParent(fossilDisplayParent);
            fossil.transform.localPosition = Vector3.zero;
            fossil.transform.localRotation = Quaternion.identity;
        }

        // Calcula bounds do fï¿½ssil para grid de cobertura
        Renderer fossilRenderer = fossil.GetComponent<Renderer>();
        if (fossilRenderer != null)
        {
            fossilBounds = fossilRenderer.bounds;
        }
        else
        {
            Debug.LogWarning("FossilCleaning: Fï¿½ssil precisa de um Renderer (ex.: SpriteRenderer) para calcular bounds!");
            fossilBounds = new Bounds(fossil.transform.position, Vector3.one * 5f); // Fallback aproximado
        }

        // Configura DirtOverlay e cria grid de sujeira se necessï¿½rio
        dirtOverlay = fossil.transform.Find("DirtOverlay");
        if (dirtOverlay != null)
        {
            CreateDirtGrid(); // Cria/atualiza sub-sprites de sujeira
        }
        else
        {
            Debug.LogWarning("FossilCleaning: 'DirtOverlay' nï¿½o encontrado no fï¿½ssil. Sem visual de sujeira (sï¿½ progress bar).");
        }

        // Reset ferramentas e grid
        for (int i = 0; i < numTools; i++)
        {
            toolsCompleted[i] = false;
        }
        currentToolIndex = -1;
        ResetCurrentGrid(); // Grid inicial vazia

        // Reset progress bar se configurada
        if (toolProgressBar != null)
        {
            toolProgressBar.value = 0f;
        }

        // Isola a cï¿½mera: Desativa main camera, ativa cleaning camera focada no fï¿½ssil
        if (cleaningCamera != null)
        {
            mainCamera.gameObject.SetActive(false); // Esconde o mundo/jogador
            cleaningCamera.gameObject.SetActive(true);
            cleaningCamera.transform.position = fossil.transform.position + cleaningCameraOffset; // Foca no fï¿½ssil (zoom/close-up)
            cleaningCamera.orthographicSize = 2f; // Ajusta zoom (menor = mais prï¿½ximo; ajuste conforme necessï¿½rio)
            cleaningCamera.cullingMask = cleaningLayerMask; // Renderiza sï¿½ UI + fï¿½ssil (configure layers no Inspector)
        }

        Debug.Log("Limpeza iniciada: Passe o mouse sobre o fï¿½ssil com cada ferramenta para cobrir 80% da ï¿½rea. Veja a sujeira sumir!");
    }

    // Cria grid de sub-sprites de sujeira dinamicamente (se nï¿½o existirem)
    private void CreateDirtGrid()
    {
        int totalCells = gridSize * gridSize;
        dirtCells = new GameObject[totalCells];

        // Remove filhos existentes se quiser recriar
        foreach (Transform child in dirtOverlay)
        {
            if (child.name.StartsWith("DirtCell_")) DestroyImmediate(child.gameObject);
        }

        // Cria novos DirtCells
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                int index = y * gridSize + x;
                GameObject cell = new GameObject("DirtCell_" + x + "_" + y);
                cell.transform.SetParent(dirtOverlay);
                cell.transform.localPosition = GetGridPosition(x, y); // Posiï¿½ï¿½o na grid
                cell.transform.localScale = Vector3.one * 0.1f;
                // Adiciona SpriteRenderer com sprite de sujeira (crie um sprite pequeno de terra)
                SpriteRenderer sr = cell.AddComponent<SpriteRenderer>();
                sr.sprite = Resources.Load<Sprite>("DirtPatch"); // Carregue um sprite de "terra" de Assets/Resources/DirtPatch.png
                if (sr.sprite == null)
                {
                    Debug.LogWarning("Crie um sprite 'DirtPatch' em Assets/Resources para visual de sujeira!");
                    // Fallback: Use cor marrom (quad simples)
                    sr.color = new Color(0.6f, 0.4f, 0.2f, 1f); // Marrom
                }
                sr.sortingOrder = 1; // Por cima do fï¿½ssil

                cell.SetActive(true); // Inicialmente visï¿½vel (sujeira)
                dirtCells[index] = cell;
            }
        }
        Debug.Log("Grid de sujeira criada: " + totalCells + " cï¿½lulas.");
    }

    // Calcula posiï¿½ï¿½o local de uma cï¿½lula na grid
    private Vector3 GetGridPosition(int x, int y)
    {
        float cellSizeX = fossilBounds.size.x / gridSize;
        float cellSizeY = fossilBounds.size.y / gridSize;
        float posX = fossilBounds.min.x + (x + 0.5f) * cellSizeX - fossilBounds.center.x;
        float posY = fossilBounds.min.y + (y + 0.5f) * cellSizeY - fossilBounds.center.y;
        return new Vector3(posX, posY, 0);
    }

    // Configura os botï¿½es das ferramentas (chamado no Awake)
    private void SetupToolButtons()
    {
        for (int i = 0; i < toolButtons.Length && i < numTools; i++)
        {
            int index = i; // Capture para lambda
            toolButtons[i].onClick.AddListener(() => SelectTool(index));
        }
    }

    // Seleï¿½ï¿½o de ferramenta (chamado pelos botï¿½es)
    private void SelectTool(int toolIndex)
    {
        if (toolIndex < 0 || toolIndex >= numTools) return;

        // Opcional: Verifique se anterior estï¿½ completa (para forï¿½ar ordem)
        if (currentToolIndex >= 0 && currentToolIndex < toolIndex - 1 && !toolsCompleted[currentToolIndex])
        {
            Debug.LogWarning("Complete a ferramenta anterior antes de selecionar a prï¿½xima!");
            return; // Bloqueia se quiser; remova para permitir qualquer ordem
        }

        currentToolIndex = toolIndex;
        ResetCurrentGrid(); // Reseta grid para nova ferramenta
        if (toolProgressBar != null)
        {
            toolProgressBar.value = 0f; // Reset progresso
        }
        Debug.Log("Ferramenta selecionada: " + toolIndex + " (" + toolButtons[toolIndex].name + "). Passe o mouse sobre o fï¿½ssil segurando LMB.");
    }

    // Marca cï¿½lulas na grid baseadas na posiï¿½ï¿½o do hit (simula pincelada) + feedback visual
    private void MarkCoverage(Vector2 hitPoint)
    {
        // Converte hitPoint para coordenadas relativas da grid (0 a gridSize-1)
        Vector2 relativePos = new Vector2(
            (hitPoint.x - fossilBounds.min.x) / fossilBounds.size.x,
            (hitPoint.y - fossilBounds.min.y) / fossilBounds.size.y
        );

        int centerX = Mathf.RoundToInt(relativePos.x * (gridSize - 1));
        int centerY = Mathf.RoundToInt(relativePos.y * (gridSize - 1));
        centerX = Mathf.Clamp(centerX, 0, gridSize - 1);
        centerY = Mathf.Clamp(centerY, 0, gridSize - 1);

        // Marca cï¿½lulas em um raio (brushRadius) ao redor do centro
        for (int x = -Mathf.RoundToInt(brushRadius); x <= Mathf.RoundToInt(brushRadius); x++)
        {
            for (int y = -Mathf.RoundToInt(brushRadius); y <= Mathf.RoundToInt(brushRadius); y++)
            {
                int gridX = centerX + x;
                int gridY = centerY + y;
                if (gridX >= 0 && gridX < gridSize && gridY >= 0 && gridY < gridSize)
                {
                    int index = gridY * gridSize + gridX;
                    currentCoverageGrid[gridX, gridY] = true;

                    // Desativa DirtCell correspondente (sujeira some)
                    if (dirtCells != null && index < dirtCells.Length && dirtCells[index] != null)
                    {
                        dirtCells[index].SetActive(false);
                    }
                }
            }
        }
    }

    // Reseta a grid de cobertura para a ferramenta atual + reativa sujeira se necessï¿½rio
    private void ResetCurrentGrid()
    {
        currentCoverageGrid = new bool[gridSize, gridSize];
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                currentCoverageGrid[x, y] = false;
            }
        }

        // Reativa todas as DirtCells para nova ferramenta
        if (dirtCells != null)
        {
            for (int i = 0; i < dirtCells.Length; i++)
            {
                if (dirtCells[i] != null)
                {
                    dirtCells[i].SetActive(true);
                }
            }
        }
    }

    // Verifica se a ferramenta atual estï¿½ completa (cobertura > threshold) + atualiza progress bar
    private void CheckToolCompletion()
    {
        if (currentToolIndex < 0) return;

        int coveredCells = 0;
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (currentCoverageGrid[x, y]) coveredCells++;
            }
        }

        float coveragePercent = (float)coveredCells / (gridSize * gridSize);
        if (toolProgressBar != null)
        {
            toolProgressBar.value = coveragePercent; // Atualiza barra em tempo real
        }

        // Log opcional de progresso (remova para produï¿½ï¿½o)
        Debug.Log("Progresso Ferramenta " + currentToolIndex + ": " + (coveragePercent * 100).ToString("F1") + "%");

        if (coveragePercent >= coverageThreshold && !toolsCompleted[currentToolIndex])
        {
            toolsCompleted[currentToolIndex] = true;
            Debug.Log("Ferramenta " + currentToolIndex + " completa! Cobertura: " + (coveragePercent * 100).ToString("F1") + "%. Escolha a prï¿½xima.");
            // Opcional: Ative visual (ex.: mude cor do botï¿½o) ou som
        }
    }

    // Verifica se todas as ferramentas foram completas
    private void CheckCompletion()
    {
        bool allToolsComplete = true;
        for (int i = 0; i < numTools; i++)
        {
            if (!toolsCompleted[i])
            {
                allToolsComplete = false;
                break;
            }
        }

        if (allToolsComplete)
        {
            CompleteCleaning();
        }
    }

    // Conclui a limpeza (chamado quando allToolsComplete)
    private void CompleteCleaning()
    {
        cleaningUI.SetActive(false);

        // Reativa movimento do jogador
        if (interaction != null)
        {
            interaction.PausePlayerMovement(false); // Reativa inputs e movimento
        }

        // Restaura a cï¿½mera: Desativa cleaning camera, ativa main camera
        if (cleaningCamera != null)
        {
            cleaningCamera.gameObject.SetActive(false);
            mainCamera.gameObject.SetActive(true);
            // Reset posiï¿½ï¿½o da cleaning camera se necessï¿½rio (para prï¿½xima vez)
            cleaningCamera.orthographicSize = mainCamera.orthographicSize; // Restaura zoom original
        }

        if (currentFossil != null)
        {
            // Opcional: Destrua ou esconda o fï¿½ssil sujo, mostre versï¿½o limpa
            // Destroy(currentFossil); // Ou ative Sprite limpo
            // Reset scale se parentado
            currentFossil.transform.localScale = Vector3.one;
        }

        // Liga de volta ao InteractionFase3
        if (interaction != null)
        {
            interaction.EndCleaning();
        }

        currentFossil = null;
        currentToolIndex = -1;
        Debug.Log("Todas as ferramentas completas! Fï¿½ssil limpo. Jogo volta ao normal.");
        interaction.DestruirObjetoAtual();

        placar.PlacarFossil(1);
        player.coletando = true;
    }

    // Mï¿½todo pï¿½blico para cancelar limpeza (ex.: se o jogador sair da ï¿½rea)
    public void CancelCleaning()
    {
        CompleteCleaning(); // Ou reset sem adicionar ao inventï¿½rio
    }
}