# Data Center — Game API Reference (`Assembly-CSharp.dll`)

> **Auto-generated** by [`tools/dump-api`](./tools/dump-api) from the game's `Assembly-CSharp.dll`
> (the copy bundled in `DC-NetworkingPlus-Mod/lib/`). Do not edit by hand — regenerate after a
> game update with: `cd tools/dump-api && dotnet run -c Release -- <path-to-Assembly-CSharp.dll>`.
>
> All game types live under the `Il2Cpp` namespace at compile time (Il2CppInterop convention), so in
> code you write `using Il2Cpp;`. Data members shown below are exposed by Il2CppInterop as **properties**.
> "Methods (incl. non-public)" are listed because Harmony can patch non-public methods too.

---

## Game Bootstrap & Managers

### `MainGameManager` (class)
*: MonoBehaviour*

**Data members** *(read/write state)*

- `Sprite appTypesColorSprite1`
- `Sprite appTypesColorSprite2`
- `Sprite appTypesColorSprite3`
- `Sprite appTypesColorSprite4`
- `Il2CppReferenceArray<Sprite> appTypesLogos`
- `List<Vector3> assignedAppsLogos`
- `Coroutine autoSaveCoroutine`
- `Boolean autoSaveEnabled`
- `Single autoSaveIntervalMinutes`
- `List<Int32> availableCustomerIndices`
- `List<String> availableSubnets`
- `List<Int32> availableVlanIds`
- `Material blinkingGreenLightMaterial`
- `Material blinkingRedLightMaterial`
- `Il2CppReferenceArray<GameObject> cableSpinnerPrefab`
- `GameObject canvasCustomerChoice`
- `GameObject canvasOpenWall`
- `CarController carController`
- `Il2CppReferenceArray<CustomerItem> chosenCustomerItems`
- `ComputerShop computerShop`
- `CustomerBaseDoor customerBaseDoor`
- `Il2CppReferenceArray<CustomerBase> customerBases`
- `Il2CppReferenceArray<CustomerCard> customerCards`
- `Il2CppReferenceArray<CustomerItem> customerItems`
- `Il2CppReferenceArray<Il2CppStructArray<Int32>> defaultPortsPerServerType`
- `Transform defaultTrolleyPosition`
- `Int32 difficulty`
- `GameObject emptySfpBox`
- `Action<CallbackContext> escapePerformed`
- `List<Int32> existingCustomerIDs`
- `NetworkSwitchConfiguration firewallConfiguration`
- `FirewallConfiguration firewallConfiguration2`
- `Il2CppReferenceArray<GameObject> firewallsPrefabs`
- `Material greenLightMaterial`
- `MainGameManager instance`
- `Boolean isAllowedToSave`
- `Boolean isGamePaused`
- `Boolean isIPHintHidden`
- `Boolean isPauseMenuDisallowed`
- `Boolean isPlayerCameraDisallowed`
- `String languageXMLPath`
- `Int32 lastUsedRackPositionGlobalUID`
- `Boolean loadingFirstTime`
- `NetworkSwitchConfiguration networkSwitchConfiguration`
- `OnBuyingWall onBuyingWallEvent`
- `Transform parentUsableObjects`
- `GameObject particleSystemBrokenDevice`
- `Il2CppReferenceArray<GameObject> patchPanelsPrefabs`
- `Transform placeToRespawnLostUsableObjects`
- `Camera playerCamera`
- `GameObject prefabPushTrolleySupporterPack`
- `GameObject rackMounts`
- `GameObject rackPackedPrefab`
- `GameObject rackPrefab`
- `Material redLightMaterial`
- `NetworkSwitchConfiguration routerConfiguration`
- `RouterConfiguration routerConfiguration2`
- `Il2CppReferenceArray<GameObject> routersPrefabs`
- `Il2CppReferenceArray<GameObject> serverPrefabs`
- `SetIP setIP`
- `Il2CppReferenceArray<GameObject> sfpPrefabs`
- `Il2CppReferenceArray<GameObject> sfpsBoxedPrefab`
- `Il2CppReferenceArray<GameObject> switchesPrefabs`
- `Il2CppReferenceArray<GameObject> techniciansPrefabs`
- `TrolleyLoadingBay trolleyLoadingBay`
- `TextMeshProUGUI txtOpenWallPrice`
- `Single wallPrice`
- `GameObject walls`
- `Wall wallToBuy`
- `Single xpGainMultiplier`

**Methods** (incl. non-public — Harmony hook points)

- `IEnumerator AutoSaveCoroutine()`
- `Void Awake()`
- `Void ButtonBuyWall()`
- `Void ButtonCancelBuyWall()`
- `Void ButtonCancelCustomerChoice()`
- `Void ButtonCustomerChosen(Int32 _cardID)`
- `Void CloseAnyCanvas(Boolean isCustomerChoice)`
- `Void CloseNetworkConfigCanvas()`
- `CustomerItem CreateFallbackCustomer(CustomerItem original, Int32 customerBaseID)`
- `Sprite GetAppLogo(Int32 customerID, Int32 appID)`
- `GameObject GetCableSpinnerPrefab(Int32 prefabID)`
- `CustomerItem GetCustomerItemByID(Int32 customerID)`
- `Sprite GetCustomerLogo(Int32 customerID)`
- `Single GetCustomerTotalRequirement(CustomerItem customer)`
- `GameObject GetFirewallPrefab(Int32 firewallType)`
- `String GetFreeSubnet(Single appRequirements)`
- `Int32 GetFreeVlanId()`
- `GameObject GetPatchPanelPrefab(Int32 switchType)`
- `Il2CppStructArray<Int32> GetRequiredPortsForInternet()`
- `Il2CppStructArray<Int32> GetRequiredPortsForServer(INetworkEndpoint server)`
- `GameObject GetRouterPrefab(Int32 routerType)`
- `GameObject GetServerPrefab(Int32 serverType)`
- `GameObject GetSfpBoxPrefab(Int32 prefabID)`
- `GameObject GetSfpPrefab(Int32 prefabID)`
- `GameObject GetSwitchPrefab(Int32 switchType)`
- `Void InitializeVlanPool()`
- `Boolean IsCustomerSuitableForBase(CustomerItem customer, Int32 customerBaseID)`
- `Boolean IsSubnetValid(String subnet)`
- `Void LoadTrolleyPosition(Vector3 _position, Quaternion _rotation)`
- `Void OnApplicationQuit()`
- `Void OnDestroy()`
- `Void OnLoad()`
- `Void OpenAnyCanvas()`
- `Void RemoveUsedSubnet(String subnet)`
- `Void RemoveUsedVlanId(Int32 vlanId)`
- `Void ResetTrolleyPosition()`
- `Void RestartAutoSave()`
- `String ReturnServerNameFromType(Int32 type)`
- `Void ReturnSubnet(String subnet)`
- `String ReturnSwitchNameFromType(Int32 type)`
- `Void ReturnVlanId(Int32 vlanId)`
- `Void SetAutoSaveEnabled(Boolean enabled)`
- `Void SetAutoSaveInterval(Single minutes)`
- `Void ShowBuyWallCanvas(Wall wall)`
- `Void ShowCustomerCardsCanvas(CustomerBaseDoor _door)`
- `Void ShowFirewallConfigCanvas(Firewall firewall)`
- `Void ShowNetworkConfigCanvas(NetworkSwitch networkSwitch)`
- `Void ShowRouterConfigCanvas(Router router)`
- `Void ShuffleAvailableCustomers()`
- `Void ShuffleAvailableSubnets()`
- `Void Start()`

### `PlayerManager` (class)
*: MonoBehaviour*

**Data members** *(read/write state)*

- `AudioSource audioSource`
- `CharacterController cc`
- `AudioClip defaultActionAudioClip`
- `GameObject defaultActionDustParticle`
- `Boolean enabledMouseMovement`
- `Boolean enabledPlayerMovement`
- `Boolean enabledRayLookInteract`
- `FirstPersonController fpc`
- `Image imageWaitForAction`
- `PlayerManager instance`
- `ParticleSystem iopsGainParticleSystem`
- `Boolean isGamePaused`
- `Transform moveItemPosition`
- `Int32 numberOfObjectsInHand`
- `ObjectInHand objectInHand`
- `Il2CppReferenceArray<GameObject> objectInHandGO`
- `GameObject objectInHandPositionGO`
- `Player playerClass`
- `GameObject playerGO`
- `Boolean playerIsSitting`
- `Rope rope`
- `CinemachineCamera vcam`

**Methods** (incl. non-public — Harmony hook points)

- `Void Awake()`
- `Void ConfinedCursorforUI()`
- `Void DefaultActionEffect(Vector3 _position, Single _time)`
- `Void GainIOPSEffect()`
- `Void LockedCursorForPlayerMovement()`
- `Void PlayerStopMovement()`
- `Void Start()`
- `IEnumerator WaitForActionToFinish(Vector3 _position, Single _time)`

### `TimeController` (class)
*: MonoBehaviour*

**Data members** *(read/write state)*

- `Single currentTimeOfDay`
- `Int32 day`
- `TimeController instance`
- `OnEndOfTheDay onEndOfTheDayCallback`
- `Single secondsInFullDay`
- `Single timeMultiplier`

**Methods** (incl. non-public — Harmony hook points)

- `Void Awake()`
- `Single CurrentTimeInHours()`
- `Int32 HoursFromDate(Single _time, Int32 _day)`
- `Void OnDisable()`
- `Void Start()`
- `Boolean TimeIsBetween(Single startHour, Single endHour)`
- `Void Update()`

### `AudioManager` (class)
*: MonoBehaviour*

**Data members** *(read/write state)*

- `AudioClip AudioClipButtonClick`
- `AudioClip AudioClipButtonHover`
- `AudioClip audioClipDeviceInserted`
- `AudioClip audioClipDeviceStartup`
- `AudioClip audioClipElectronicButton`
- `Il2CppReferenceArray<AudioClip> audioClipImpacts`
- `AudioClip audioClipObjectiveEnd`
- `AudioClip audioClipObjectiveStart`
- `AudioClip audioClipOpeningBox`
- `AudioClip audioClipRackDoorOpen`
- `Il2CppReferenceArray<AudioClip> audioClipRJ45`
- `AudioClip audioClipSuccessfullyConnected`
- `AudioClip calmMusic`
- `Single calmMusicDefaultVolume`
- `AudioClip coinUse`
- `Int32 currentMusic`
- `AudioSource delayedAudioSource`
- `AudioSource effectsAudioSource`
- `AudioClip fastMusic`
- `Single fastMusicDefaultVolume`
- `AudioClip iddleMusic`
- `Single iddleMusicDefaultVolume`
- `AudioManager instance`
- `AudioMixer masterMixer`
- `AudioSource musicAudioSource`

**Methods** (incl. non-public — Harmony hook points)

- `Void Awake()`
- `IEnumerator FadeIn(AudioSource audioSource, Single FadeTime, Single finalVolume)`
- `IEnumerator FadeOut(AudioSource audioSource, Single FadeTime)`
- `IEnumerator FadeOut_FadeIn(AudioSource audioSource, Single FadeTime, Single finalVolume, AudioClip newAudioClip)`
- `Void PlayEffectAudioClip(AudioClip audioClip, Single volume, Single delayed)`
- `Void PlayRackDoorOpen()`
- `Void PlayRandomImpactClip(Single _volume)`
- `Void PlayRandomRJ45Clip()`
- `Void SetEffectsVolume(Single _volume)`
- `Void SetMasterVolume(Single _volume)`
- `Void SetMusic(Int32 _clipUID)`
- `Void SetMusicVolume(Single _volume)`
- `Void SetRacksVolume(Single _volume)`

### `InputManager` (class)
*: MonoBehaviour*

**Data members** *(read/write state)*

- `Action<CallbackContext> checkControlsAction`
- `InputDevice device`
- `InputController inputActions`
- `OnControlChange onControlChange`
- `Action rebindCanceled`
- `Action rebindComplete`
- `Action<InputAction, Int32> rebindStarted`

**Methods** (incl. non-public — Harmony hook points)

- `static Void add_rebindCanceled(Action value)`
- `static Void add_rebindComplete(Action value)`
- `static Void add_rebindStarted(Action<InputAction, Int32> value)`
- `Void Awake()`
- `Void CheckCurrentControls(CallbackContext ctx)`
- `static Void ConfinedCursorforUI()`
- `static Void DoRebind(InputAction actionToRebind, Int32 bindingIndex, TextMeshProUGUI statusText, Boolean allCompositeParts, Boolean excludeMouse)`
- `static Void ForceMousePositionToCenterOfGameWindow()`
- `static String GetBindingName(String actionName, Int32 bindingIndex)`
- `static Void LoadAllBindingOverrides()`
- `static Void LoadBindingOverride(String actionName)`
- `static Void LockedCursorForPlayerMovement()`
- `Void OnDestroy()`
- `static Void remove_rebindCanceled(Action value)`
- `static Void remove_rebindComplete(Action value)`
- `static Void remove_rebindStarted(Action<InputAction, Int32> value)`
- `static Void ResetBinding(String actionName, Int32 bindingIndex)`
- `static Void SaveBindingOverride(InputAction action)`
- `static Void StartRebind(String actionName, Int32 bindingIndex, TextMeshProUGUI statusText, Boolean excludeMouse)`

### `SteamManager` (class)
*: MonoBehaviour*

**Data members** *(read/write state)*

- `Boolean Initialized` *(read-only)*
- `SteamManager Instance` *(read-only)*
- `Boolean s_EverInitialized`
- `SteamManager s_instance`

**Methods** (incl. non-public — Harmony hook points)

- `Void Awake()`
- `static Void InitOnPlayMode()`
- `Void OnDestroy()`
- `Void OnEnable()`
- `static Void SteamAPIDebugTextHook(Int32 nSeverity, StringBuilder pchDebugText)`
- `Void Update()`

### `TechnicianManager` (class)
*: MonoBehaviour*

**Data members** *(read/write state)*

- `List<CommandCenterOperator> commandCenterOperator`
- `Single DISPATCH_INTERVAL`
- `Coroutine dispatchCoroutine`
- `Il2CppStructArray<Int32> hiredTechnicians`
- `TechnicianManager instance`
- `Queue<RepairJob> jobQueue`
- `Int32 lastAssignedIndex`
- `Single lastDispatchTime`
- `Il2CppStructArray<Int32> openedDumsterAreas`
- `Queue<RepairJob> pendingDispatches`
- `Int32 QueuedJobCount` *(read-only)*
- `List<Technician> technicians`
- `Il2CppReferenceArray<Transform> transformContainer`
- `Il2CppReferenceArray<Transform> transformDeviceSpawnPosition`
- `Il2CppReferenceArray<Transform> transformDumpster`
- `Il2CppReferenceArray<Transform> transformIdle`

**Methods** (incl. non-public — Harmony hook points)

- `Void AddTechnician(Technician technician)`
- `Void Awake()`
- `Void EnqueueDispatch(RepairJob job)`
- `Void FireTechnician(Int32 technicianID)`
- `List<RepairJob> GetActiveJobs()`
- `Int32 GetClosestOpenedDumpsterIndex(Vector3 position)`
- `List<RepairJob> GetQueuedJobs()`
- `Boolean IsDeviceAlreadyAssigned(NetworkSwitch networkSwitch, Server server)`
- `Void OnDestroy()`
- `Void OnLoadingStarted()`
- `Void OpenDumpsterArea(Int32 areaID)`
- `IEnumerator ProcessDispatchQueue()`
- `Void RequestNextJob(Technician technician)`
- `Void RestoreJobQueue(List<RepairJobSaveData> savedJobs)`
- `Void SendTechnician(NetworkSwitch networkSwitch, Server server)`

### `DeviceTimerManager` (class)
*: MonoBehaviour*

**Data members** *(read/write state)*

- `List<ITimedDevice> activeDevices`
- `DeviceTimerManager instance`
- `List<ITimedDevice> tickSnapshot`

**Methods** (incl. non-public — Harmony hook points)

- `Void Awake()`
- `Void OnEnable()`
- `Void Register(ITimedDevice device)`
- `IEnumerator TimerLoop()`
- `Void Unregister(ITimedDevice device)`

## Player & Economy

### `Player` (class)
*: MonoBehaviour*

**Data members** *(read/write state)*

- `InputController inputctrl`
- `Single money`
- `Single previousCoins`
- `Single reputation`
- `Vector3 respawnPos`
- `Transform targetSpawn`
- `Single xp`

**Methods** (incl. non-public — Harmony hook points)

- `Void CheckFallsThroughMap()`
- `Void DropAllItems()`
- `Void LoadPlayer(PlayerData data)`
- `Void Start()`
- `IEnumerator TurnOnCharacterControllerDelayed()`
- `Boolean UpdateCoin(Single _coinChhangeAmount, Boolean withoutSound)`
- `Void UpdateReputation(Single amount)`
- `Boolean UpdateXP(Single amount)`
- `Void WarpPlayer(Vector3 _position, Quaternion _rotation)`

### `PlayerData` (class)

**Data members** *(read/write state)*

- `List<Int32> activeObjectives`
- `Single coins`
- `Il2CppStructArray<Single> position`
- `Single reputation`
- `Single xp`

## Network Core

### `NetworkMap` (class)
*: MonoBehaviour*

**Data members** *(read/write state)*

- `Dictionary<String, List<String>> adjacencyList`
- `Dictionary<String, Server> brokenServers`
- `Dictionary<String, NetworkSwitch> brokenSwitches`
- `Dictionary<Int32, ValueTuple<String, String>> cableConnections`
- `Dictionary<Int32, CustomerBase> customerBases`
- `Dictionary<String, Device> devices`
- `NetworkMap instance`
- `Dictionary<Int32, LACPGroup> lacpGroups`
- `Int32 nextLACPGroupId`
- `Dictionary<String, INetworkEndpoint> servers`
- `Dictionary<String, HashSet<String>> switchConnections`
- `Dictionary<String, NetworkSwitch> switches`
- `HashSet<Int32> usedAsns`
- `HashSet<String> usedSubnets`
- `HashSet<Int32> usedVlanIDs`

**Methods** (incl. non-public — Harmony hook points)

- `Void AddBrokenServer(Server server)`
- `Void AddBrokenSwitch(NetworkSwitch networkSwitch)`
- `Void AddDevice(String name, TypeOfLink type, Int32 customerID)`
- `Void AddSwitchConnection(String switchName, String deviceName)`
- `Void Awake()`
- `Void BacktrackPaths(String node, Dictionary<String, List<String>> predecessors, List<String> working, List<List<String>> result, HashSet<String> visited)`
- `Void ClearMap()`
- `Void Connect(String from, String to)`
- `Int32 CreateLACPGroup(String deviceA, String deviceB, List<Int32> cableIds)`
- `Void Disconnect(String from, String to)`
- `Dictionary<String, List<String>> FindAllReachablePathsFrom(String startDevice)`
- `Dictionary<String, List<List<String>>> FindAllServerPathsFromCustomer(Int32 customerId)`
- `List<List<String>> FindAllShortestPaths(IEnumerable<String> sources, String targetDevice)`
- `List<List<String>> FindAllShortestPathsBetween(String startDevice, String targetDevice)`
- `List<List<String>> FindAllShortestPathsFromCustomer(Int32 customerId, String targetDevice)`
- `String GenerateDeviceName(TypeOfLink type, Vector3 position)`
- `IEnumerable<Server> GetAllBrokenServers()`
- `IEnumerable<NetworkSwitch> GetAllBrokenSwitches()`
- `IEnumerable<CustomerBase> GetAllCustomerBases()`
- `List<Device> GetAllDevices()`
- `IEnumerable<Firewall> GetAllFirewalls()`
- `Dictionary<Int32, LACPGroup> GetAllLACPGroups()`
- `IEnumerable<NetworkSwitch> GetAllNetworkSwitches()`
- `IEnumerable<Router> GetAllRouters()`
- `IEnumerable<INetworkEndpoint> GetAllServers()`
- `CustomerBase GetCustomerBase(Int32 customerId)`
- `Device GetDevice(String name)`
- `LACPGroup GetLACPGroupBetween(String deviceA, String deviceB)`
- `LACPGroup GetLACPGroupForCable(Int32 cableId)`
- `Il2CppStructArray<Int32> GetNumberOfDevices()`
- `INetworkEndpoint GetServer(String serverId)`
- `NetworkSwitch GetSwitchById(String switchId)`
- `Boolean IpInCidr(String ip, String cidr)`
- `Boolean IsInternetDevice(String deviceId)`
- `Boolean IsIpAddressDuplicate(String ip, Server serverToExclude)`
- `Boolean IsIpInsideSubnetExcludingReserved(String ip, String subnetCidr, ref Boolean isReserved)`
- `Boolean IsPatchPanelPort(String deviceName)`
- `Boolean IsPhysicallyReachable(String fromDeviceId, String toDeviceId)`
- `Boolean IsTrafficAllowedOnRoute(List<String> path, String sourceIp, String destIp, IReadOnlyList<Int32> requiredPorts, Protocol protocol, Dictionary<ValueTuple<String, String>, CableInfo> cablePairLookup, String sourceSubnet)`
- `Boolean IsTrafficAllowedOnRouteForPort(List<String> path, String sourceIp, String destIp, Int32 networkPort, Protocol protocol, Dictionary<ValueTuple<String, String>, CableInfo> cablePairLookup, String sourceSubnet)`
- `Boolean IsVlanAllowedOnRoute(List<String> path, Int32 vlanId, Dictionary<ValueTuple<String, String>, CableInfo> cablePairLookup)`
- `Boolean IsVlanAllowedOnRouteSegmented(List<String> path, Int32 appVlanId, String serverIp, Dictionary<ValueTuple<String, String>, CableInfo> cablePairLookup)`
- `String PrintNetworkMap()`
- `Void RegisterCableConnection(Int32 cableId, Vector3 startPos, Vector3 endPos, TypeOfLink startType, TypeOfLink endType, String startSwitchID, String endSwitchID, Int32 startCustomerID, Int32 endCustomerID, String startServerID, String endServerID)`
- `Void RegisterCustomerBase(CustomerBase customerBase)`
- `Void RegisterEndpoint(INetworkEndpoint endpoint)`
- `Void RegisterServer(Server server)`
- `Void RegisterSwitch(NetworkSwitch networkSwitch)`
- `Void RemapDeviceId(String oldId, String newId)`
- `Void RemoveBrokenServer(String serverId)`
- `Void RemoveBrokenSwitch(String switchId)`
- `Void RemoveCableConnection(Int32 cableId, Boolean preserveLACP)`
- `Void RemoveCableFromLACPGroups(Int32 cableId)`
- `Void RemoveDevice(String name)`
- `Void RemoveIsolatedDevice(String deviceName)`
- `Void RemoveLACPGroup(Int32 groupId)`
- `String ResolveRealSource(String node, Dictionary<String, List<String>> predecessors)`
- `String ResolveThroughPatchPanel(String patchPanelPort, String fromDevice)`
- `Void SetLACPGroups(Dictionary<Int32, LACPGroup> groups)`
- `static Boolean TryParseIp(String ipString, ref UInt32 value)`
- `Int32 TryTranslateVlanAtRouter(Router router, Int32 currentVlan, String serverIp)`
- `Void UpdateCustomerServerCountAndSpeed(Int32 customerId, Int32 serverCount, Single speed)`
- `Void UpdateDeviceCustomerID(String deviceName, Int32 customerID)`

### `INetworkEndpoint` (class)
*: Il2CppObjectBase*

**Data members** *(read/write state)*

- `Int32 appID` *(read-only)*
- `Single currentProcessingSpeed`
- `Int32 CustomerID` *(read-only)*
- `String IP` *(read-only)*
- `Boolean isOn` *(read-only)*
- `Single maxProcessingSpeed` *(read-only)*
- `String ServerID` *(read-only)*
- `Int32 serverType` *(read-only)*

**Methods** (incl. non-public — Harmony hook points)

- `Void UpdateAppID(Int32 appID)`

### `ITimedDevice` (class)
*: Il2CppObjectBase*

**Methods** (incl. non-public — Harmony hook points)

- `Void TickTimer()`

## Devices

### `Server` (class)
*: UsableObject*

**Data members** *(read/write state)*

- `List<CableLink> activeLinks`
- `Int32 appID`
- `Image appLogo`
- `Il2CppReferenceArray<CableLink> cablelinks`
- `GameObject canvas`
- `Single currentProcessingSpeed`
- `Image customerLogo`
- `Int32 eolTime`
- `Int32 existingErrorSigns`
- `Int32 existingWarningSigns`
- `Boolean hasInitialized`
- `String IP`
- `Boolean isBroken`
- `Boolean isOn`
- `Boolean isWarningCleared`
- `Int32 lastDisplayedEolMinute`
- `String lastDisplayedLabel`
- `Single lastDisplayedMaxSpeed`
- `Single lastDisplayedProcessingSpeed`
- `Single maxProcessingSpeed`
- `Renderer powerButton`
- `Single previousProcessingSpeed`
- `StringBuilder sb`
- `String ServerID`
- `Int32 serverType`
- `Int32 timeToBrake`
- `TextMeshProUGUI txtIP`
- `TextMeshProUGUI txtServerScreen`

**Methods** (incl. non-public — Harmony hook points)

- `static Void AppendEolTime(StringBuilder builder, Int32 eolSeconds)`
- `Void Awake()`
- `Void ButtonClickChangeCustomer(Boolean forward)`
- `Void ButtonClickChangeIP()`
- `Void ClearAppLogo()`
- `Void ClearErrorSign()`
- `Void ClearWarningSign(Boolean isPreserved)`
- `Boolean ContainsDuplicateServerID(String candidate)`
- `String GenerateUniqueServerId()`
- `Int32 GetCustomerID()`
- `Int32 GetNextCustomerID(Int32 currentCustomerID, Boolean forward)`
- `Void INetworkEndpoint_UpdateAppID(Int32 appID)`
- `Void InteractOnClick()`
- `Void InteractOnHover(RaycastHit hit)`
- `Boolean IsAnyCableConnected()`
- `Void ItIsBroken()`
- `Void OnDestroy()`
- `Void OnLoadingComplete()`
- `Void OnLoadingStarted()`
- `Void PowerButton(Boolean forceState)`
- `Void RegisterLink(CableLink link)`
- `Void RepairDevice()`
- `Void ServerInsertedInRack(ServerSaveData serverSaveData)`
- `Void SetIP(String _ip)`
- `Void SetPowerLightMaterial(Material material)`
- `Void Start()`
- `Void TickTimer()`
- `Void TurnOffCommonFunctions()`
- `Void TurnOnCommonFunction()`
- `Void UnregisterLink(CableLink link)`
- `Void UpdateAppID(Int32 _appID)`
- `Void UpdateCustomer(Int32 newCustomerID)`
- `Void UpdateServerScreenUI()`
- `Boolean ValidateRackPosition()`

### `NetworkSwitch` (class)
*: UsableObject*

**Data members** *(read/write state)*

- `Il2CppReferenceArray<CableLink> cableLinkSwitchPorts`
- `GameObject canvas`
- `Dictionary<Int32, HashSet<Int32>> disallowedVlansPerPort`
- `Int32 eolTime`
- `Int32 existingErrorSigns`
- `Int32 existingWarningSigns`
- `Boolean isBroken`
- `Boolean isOn`
- `Boolean isWarningCleared`
- `Int32 lastDisplayedEolMinute`
- `String lastDisplayedLabel`
- `Renderer powerButton`
- `StringBuilder sb`
- `String switchId`
- `Int32 switchType`
- `HashSet<Int32> temporarilyDisconnectedCables`
- `Int32 timeToBrake`
- `TextMeshProUGUI txtScreen`

**Methods** (incl. non-public — Harmony hook points)

- `static Void AppendEolTime(StringBuilder builder, Int32 eolSeconds)`
- `Void Awake()`
- `Void ButtonShowNetworkSwitchConfig()`
- `Void ClearErrorSign()`
- `Void ClearWarningSign(Boolean isPreserved)`
- `Boolean ContainsDuplicateSwitchId(String candidate)`
- `Void DisconnectCables()`
- `Void DisconnectCablesWhenSwitchIsOff()`
- `String GenerateUniqueSwitchId()`
- `Dictionary<Int32, HashSet<Int32>> GetAllDisallowedVlans()`
- `List<ValueTuple<String, Int32>> GetConnectedDevices()`
- `String GetDeviceId()`
- `HashSet<Int32> GetDisallowedVlans(Int32 portIndex)`
- `Int32 GetPortIndexForCable(Int32 cableId)`
- `Void HandleNewCableWhileOff(Int32 cableId)`
- `Void InteractOnClick()`
- `Void InteractOnHover(RaycastHit hit)`
- `Boolean IsAnyCableConnected()`
- `Boolean IsVlanAllowedOnCable(Int32 cableId, Int32 vlanId)`
- `Boolean IsVlanAllowedOnPort(Int32 portIndex, Int32 vlanId)`
- `Void ItIsBroken()`
- `Void OnDestroy()`
- `Void PatchStaleSwitchId(ref CableEndpoint endpoint)`
- `Void PowerButton(Boolean forceState)`
- `Void ReconnectCables()`
- `Void RepairDevice()`
- `Void SetDisallowedVlansPerPort(Dictionary<Int32, HashSet<Int32>> data)`
- `Void SetPowerLightMaterial(Material material)`
- `Void SetVlanAllowed(Int32 portIndex, Int32 vlanId)`
- `Void SetVlanDisallowed(Int32 portIndex, Int32 vlanId)`
- `Void Start()`
- `Void SwitchInsertedInRack(SwitchSaveData switchSaveData)`
- `Void TickTimer()`
- `Void TurnOffCommonFunctions()`
- `Void TurnOnCommonFunction()`
- `Void UpdateScreenUI()`
- `Boolean ValidateRackPosition()`

### `Router` (class)
*: NetworkSwitch*

**Data members** *(read/write state)*

- `Int32 asn`
- `List<SubnetRoute> routingTable`

**Methods** (incl. non-public — Harmony hook points)

- `Boolean AddRoute(Int32 sourceVlanId, String subnetCidr, Int32 targetVlanId, String targetIp)`
- `Void ApplyRouteToCustomerBases(SubnetRoute route)`
- `Void ButtonShowNRouterSwitchConfig()`
- `Void OnDestroy()`
- `Void ReapplyAllRoutes()`
- `Void RemoveRoute(Int32 sourceVlanId)`
- `Int32 ResolveTargetVlan(String serverIp)`
- `Void SwitchInsertedInRack(SwitchSaveData switchSaveData)`
- `Void SyncRoutesWithSameAsn()`
- `Void WithdrawRouteFromCustomerBases(SubnetRoute route)`

### `Firewall` (class)
*: NetworkSwitch*

**Data members** *(read/write state)*

- `String clusterIP`
- `List<FilterRule> filterRules`

**Methods** (incl. non-public — Harmony hook points)

- `Void AddRule(Int32 portIndex, String sourceIpCidr, String destIpCidr, Int32 networkPort, Protocol protocol, Boolean bidirectional, Boolean allow)`
- `Void BroadcastRulesToCluster()`
- `Void ButtonShowFirewallConfig()`
- `Void InsertedInRack(SwitchSaveData saveData)`
- `Boolean IsTrafficAllowed(Int32 portIndex, Int32 vlanId, String sourceIp, String destIp, Int32 networkPort, Protocol protocol)`
- `Boolean MatchesCidr(String ip, String cidr)`
- `Void OnConnectivityRestored()`
- `Void RemoveRule(Int32 portIndex, Int32 vlanId, String sourceIpCidr, String destIpCidr, Int32 networkPort)`
- `Void SyncRulesFromCluster()`

### `Rack` (class)
*: MonoBehaviour*

**Data members** *(read/write state)*

- `Boolean arePositionTurnedOff`
- `AudioSource audioSource`
- `Renderer buttonRackPositionsRendererer`
- `AudioSource effectAudioSource`
- `Il2CppStructArray<Int32> isPositionUsed`
- `Il2CppReferenceArray<RackPosition> positions`
- `RackMount rackMount`
- `Single targetVolume`

**Methods** (incl. non-public — Harmony hook points)

- `Void Awake()`
- `Void ButtonDisablePositionsInRack()`
- `Void ButtonUnmountRack()`
- `Void InitializeLoadedRack(Il2CppStructArray<Int32> loadedPositions)`
- `Boolean IsPositionAvailable(Int32 index, Int32 sizeInU)`
- `Void MarkPositionAsUnused(Int32 index, Int32 sizeInU)`
- `Void MarkPositionAsUsed(Int32 index, Int32 sizeInU)`
- `Void OnDestroy()`
- `Void OnLoad()`
- `Void SetDisablePositionsButtonMaterial(Material material)`
- `Void Start()`
- `IEnumerator UnmountRack()`
- `Void UpdateAudioVolume()`

### `RackMount` (class)
*: Interact*

**Data members** *(read/write state)*

- `Material disolveMaterial`
- `Boolean isRackInstantiated`
- `Material originalRackMaterial`
- `Outlinable outlineEffect`

**Methods** (incl. non-public — Harmony hook points)

- `Void ApplyMaterialToLODs(GameObject rackGO, Material mat)`
- `Void Awake()`
- `Void CheatInsertRack(GameObject go, Int32 type)`
- `IEnumerator InstallRack(Boolean cheat, Int32 type)`
- `GameObject InstantiateRack(InteractObjectData saveData)`
- `Void InteractOnClick()`
- `Void InteractOnHover(RaycastHit hit)`
- `Void OnDestroy()`
- `Void OnHoverOver()`
- `Void OnLoad()`

### `RackDoor` (class)
*: Interact*

**Data members** *(read/write state)*

- `BoxCollider boxCollider`
- `Vector3 initialRotation`
- `Boolean isOpened`
- `Single openDuration`
- `Vector3 openRotation`
- `Outlinable outlineEffect`

**Methods** (incl. non-public — Harmony hook points)

- `Void Awake()`
- `IEnumerator DelayedTrigger()`
- `Void InteractOnClick()`
- `Void InteractOnHover(RaycastHit hit)`
- `Void OnHoverOver()`

### `RackPosition` (class)
*: Interact*

**Data members** *(read/write state)*

- `Outlinable outlineEffect`
- `Int32 positionIndex`
- `Rack rack`
- `Int32 rackPosGlobalUID`
- `Dictionary<Int32, RackPosition> registry`

**Methods** (incl. non-public — Harmony hook points)

- `Void Awake()`
- `static RackPosition GetByUID(Int32 uid)`
- `IEnumerator InsertItemInRack()`
- `Void InteractOnClick()`
- `Void InteractOnHover(RaycastHit hit)`
- `Boolean IsAllowedItem(Boolean checkAvailability)`
- `Void OnDestroy()`
- `Void OnHoverOver()`
- `Void SecondActionOnClick()`
- `Void SetUID(Int32 uid)`
- `Void SetUsed(Boolean used)`

### `CableLink` (class)
*: Interact*

**Data members** *(read/write state)*

- `Int32 cableIDsOnLink`
- `Single connectionSpeed`
- `Int32 CustomerID`
- `SFPModule insertedSFP`
- `Boolean isEndPoint`
- `Boolean isFibrePort`
- `Boolean isSFPPort`
- `Boolean isStartOrEnd`
- `Outlinable outlineEffect`
- `PatchPanel parentPatchPanel`
- `Server parentServer`
- `NetworkSwitch parentSwitch`
- `Transform ropeAttachPoint`
- `Single ropeForwardOffset`
- `Single sfpForwardOffset`
- `Int32 sfpTypeInserted`
- `Int32 sfpTypeSupported`
- `String switchID`
- `TypeOfLink typeOfLink`

**Methods** (incl. non-public — Harmony hook points)

- `List<Int32> CollectPatchPanelChainCables(Int32 startCableId)`
- `Void CreateRopeAttachPoint()`
- `Transform GetRopeAttachPoint()`
- `Void InsertSFP(Single speed, Int32 type, SFPModule module)`
- `Void InteractOnClick()`
- `Void InteractOnHover(RaycastHit hit)`
- `Boolean IsAllowedToDoSecondAction()`
- `Void LabelActionOnClick()`
- `Void OnHoverOver()`
- `Void RemoveSFP()`
- `Void SecondActionOnClick()`
- `Void SetConnectionSpeed(Single speed)`
- `Void Start()`

## Device Configuration UIs

### `RouterConfiguration` (class)
*: MonoBehaviour*

**Data members** *(read/write state)*

- `Transform addedRoutesParent`
- `String mask`
- `Router router`
- `GameObject routerRow`
- `Int32 sourceVlan`
- `String subnet`
- `String targetIp`
- `Int32 targetVlan`
- `TextMeshProUGUI textAsn`
- `TextMeshProUGUI textMask`
- `TextMeshProUGUI textSourceVlan`
- `TextMeshProUGUI textSubnet`
- `TextMeshProUGUI textTargetVlan`

**Methods** (incl. non-public — Harmony hook points)

- `Void ButtonAddRoute()`
- `Void ButtonClearText()`
- `Void ButtonSetAsn()`
- `Void ButtonSetMask()`
- `Void ButtonSetSubnet()`
- `Void ButtonSetTarget()`
- `Void ButtonSetVLAN()`
- `Void CreateRouteRowUI(Int32 sVlan, Int32 tVlan, String subnetCidr, String tIp)`
- `Void OnDisable()`
- `Void OpenConfig(Router _router)`
- `Void StartEditRoute(Int32 sourceVlanId, GameObject rowObj)`

### `FirewallConfiguration` (class)
*: MonoBehaviour*

**Data members** *(read/write state)*

- `Transform addedRulesParent`
- `Boolean allow`
- `Boolean bidirectional`
- `String clusterIP`
- `String destIp`
- `Firewall firewall`
- `GameObject firewallRuleRowPrefab`
- `String networkPortInput`
- `Protocol protocol`
- `String sourceIp`
- `TextMeshProUGUI textAllow`
- `TextMeshProUGUI textBidirectional`
- `TextMeshProUGUI textClusterIP`
- `TextMeshProUGUI textDestIp`
- `TextMeshProUGUI textPort`
- `TextMeshProUGUI textProtocol`
- `TextMeshProUGUI textSourceIp`

**Methods** (incl. non-public — Harmony hook points)

- `Void ButtonAddRule()`
- `Void ButtonConfigureClusterIP()`
- `Void ButtonCycleProtocol()`
- `Void ButtonSetDestIp()`
- `Void ButtonSetPort()`
- `Void ButtonSetSourceIp()`
- `Void ButtonToggleAllow()`
- `Void ButtonToggleBidirectional()`
- `Void CreateRuleRowUI(FilterRule rule)`
- `Void OnDisable()`
- `Void OnEnable()`
- `Void OpenConfig(Firewall _firewall)`
- `Void RefreshAllowLabel()`
- `Void RefreshBidirectionalLabel()`
- `Void RefreshProtocolLabel()`
- `Void StartEditRule(FilterRule rule, GameObject rowObj)`

### `NetworkSwitchConfiguration` (class)
*: MonoBehaviour*

**Data members** *(read/write state)*

- `Transform allowedVLANsButtons`
- `NetworkSwitch currentNetworkSwitch`
- `Il2CppReferenceArray<CableLink> currentPorts`
- `Transform disallowedVLANsButtons`
- `Image imagePowerButton`
- `TextMeshProUGUI labelText`
- `Transform parentObjectForPortText`
- `Dictionary<ValueTuple<String, Int32>, ValueTuple<CableLink, PatchPanel>> patchPanelLinkCache`
- `Il2CppReferenceArray<TextMeshProUGUI> portInformation`
- `HashSet<Int32> selectedPortIndices`
- `TextMeshProUGUI txtPortIDVlanTitle`
- `GameObject vlanButtonPrefab`

**Methods** (incl. non-public — Harmony hook points)

- `Void Awake()`
- `Void BuildPatchPanelCache()`
- `Void ButtonEditLabel()`
- `Void ButtonPower()`
- `Void ClearVLANDisplay()`
- `Void ClickPort(Int32 i)`
- `Void CloseConfig()`
- `Void CreateLACP()`
- `ButtonExtended CreateVLANButtonMulti(Int32 vlanId, List<Int32> portIndices, Transform parent)`
- `static String GetDevicePrefix(String deviceId)`
- `HashSet<Int32> GetVisibleVLANs()`
- `String NormalizeDeviceKey(String deviceName)`
- `Void OnEndEditingInputText(String s)`
- `Void OpenConfig(NetworkSwitch networkSwitch)`
- `Void RefreshPortDisplay()`
- `Void RefreshVLANDisplayForSelection(Int32 selectVlanId)`
- `Void RemoveLACP()`
- `String ResolveOtherEndpoint(ValueTuple<String, String> conn, String primaryId, String fallbackId)`
- `String ResolveRemoteDevice(CableLink port)`
- `Void ToggleVLANMulti(List<Int32> portIndices, Int32 vlanId)`
- `ValueTuple<String, List<Int32>> TraversePatchPanels(CableLink port)`

### `FirewallRuleRow` (class)
*: MonoBehaviour*

**Data members** *(read/write state)*

- `ButtonExtended buttonDelete`
- `ButtonExtended buttonEdit`
- `FirewallConfiguration config`
- `Firewall firewall`
- `FilterRule rule`
- `TextMeshProUGUI textAllow`
- `TextMeshProUGUI textBidirectional`
- `TextMeshProUGUI textDestIp`
- `TextMeshProUGUI textPort`
- `TextMeshProUGUI textProtocol`
- `TextMeshProUGUI textSourceIp`

**Methods** (incl. non-public — Harmony hook points)

- `Void Initialize(FirewallConfiguration _config, Firewall _firewall, FilterRule _rule)`
- `Void OnDeleteClicked()`
- `Void OnEditClicked()`

### `RouterRouteRow` (class)
*: MonoBehaviour*

**Data members** *(read/write state)*

- `ButtonExtended buttonDelete`
- `ButtonExtended buttonEdit`
- `RouterConfiguration config`
- `Router router`
- `Int32 sourceVlan`
- `Int32 targetVlan`
- `TextMeshProUGUI textGatewayIP`
- `TextMeshProUGUI textSubnetCidr`
- `TextMeshProUGUI textTargetVLAN`
- `TextMeshProUGUI textVLAN`

**Methods** (incl. non-public — Harmony hook points)

- `Void Initialize(RouterConfiguration _config, Router _router, Int32 _sourceVlan, Int32 _targetVlan, String subnetCidr, String _gateway, String _targetIp)`
- `Void OnDeleteClicked()`
- `Void OnEditClicked()`

## Shop & Items

### `ComputerShop` (class)
*: Interact*

**Data members** *(read/write state)*

- `Vector3 additionalSpawnPosForPatchpanel`
- `Vector3 additionalSpawnPosForSFPBox`
- `GameObject assetManagementScreen`
- `GameObject balanceSheetScreen`
- `ButtonExtended buttonCheckOut`
- `GameObject canvasComputerShop`
- `List<ShopCartItem> cartUIItems`
- `Int32 currentPrice`
- `Int32 currentSpawnIndex`
- `Action<CallbackContext> escapePerformed`
- `FlexibleColorPicker flexibleColorPicker`
- `GateLever gateLever`
- `GameObject hireScreen`
- `Boolean isPendingColorPurchase`
- `Il2CppStructArray<Int32> itemsSpawnsInUse`
- `GameObject mainScreen`
- `GameObject networkMapScreen`
- `Outlinable outlineEffect`
- `Transform parentForShopCartItems`
- `ShopCartItem pendingCartItem`
- `String pendingDisplayName`
- `Int32 pendingItemID`
- `ObjectInHand pendingItemType`
- `Int32 pendingPrice`
- `GameObject shopCartItemPrefab`
- `GameObject shopItemParent`
- `Il2CppReferenceArray<ShopItem> shopItems`
- `GameObject shopScreen`
- `Dictionary<Int32, Int32> spawnedItemPositions`
- `Dictionary<Int32, GameObject> spawnedItems`
- `GameObject srScreen`
- `TextMeshProUGUI text_totalPrice`
- `TextMeshProUGUI textNetworkMap`
- `Transform transformItemSpawn`
- `Il2CppReferenceArray<Transform> transformProductItemsSpawns`
- `Int32 uniqueID`
- `Boolean wasTutorialShownAlready`

**Methods** (incl. non-public — Harmony hook points)

- `Void ApplyColorToSpawnedItem(Int32 uid, Color color, ObjectInHand itemType)`
- `Void Awake()`
- `Void ButtonAssetManagementScreen()`
- `Void ButtonBalanceSheetScreen()`
- `Void ButtonBuyShopItem(Int32 itemID, Int32 price, ObjectInHand itemType, String displayName, Boolean isCustomColor)`
- `Void ButtonCancel()`
- `Void ButtonCancelColorPicker()`
- `Void ButtonCheckOut()`
- `Void ButtonChosenColor()`
- `Void ButtonClear()`
- `Void ButtonHireScreen()`
- `Void ButtonNetworkMap()`
- `Void ButtonReturnMainScreen()`
- `Void ButtonShopScreen()`
- `Void ButtonSRScreen()`
- `Void BuyAnotherItem(Int32 itemID, Int32 price, ObjectInHand itemType, ShopCartItem cartItem)`
- `Void BuyNewItem(Int32 itemID, Int32 price, ObjectInHand itemType, String displayName)`
- `Void CleanUpShop()`
- `Void ClearTrackingWithoutDestroying()`
- `Void CloseShop()`
- `Void DestroyAllSpawnedItems()`
- `Void FreeUpSpawnPoint(Int32 spawnIndex)`
- `Dictionary<Int32, Transform> GetNextAvailableSpawnPoint()`
- `GameObject GetPrefabForItem(Int32 itemID, ObjectInHand itemType)`
- `Void HandleObjectives(ObjectInHand itemType)`
- `Void InteractOnClick()`
- `Void InteractOnHover(RaycastHit hit)`
- `Void OnDestroy()`
- `Void OnHoverOver()`
- `Void OnLoad()`
- `Void OpenColorPicker()`
- `Void RemoveCartUIItem(ShopCartItem cartItem)`
- `Void RemoveSpawnedItem(Int32 uid)`
- `Void SelectNextAvailable(Int32 removedIndex)`
- `Void SpawnNewCartItem(Int32 itemID, Int32 price, ObjectInHand itemType, String displayName, Nullable<Color> chosenColor)`
- `Nullable<Int32> SpawnPhysicalItem(GameObject prefab, Int32 price, ObjectInHand itemType)`
- `Void UnlockFromSave(Dictionary<String, Boolean> savedStates)`
- `Void UpdateCartTotal()`

### `ShopItem` (class)
*: MonoBehaviour*

**Data members** *(read/write state)*

- `ButtonExtended buttonExtended`
- `String guid`
- `Boolean isUnlocked`
- `String itemDisplayName`
- `Image itemIcon`
- `ShopItemSO shopItemSO`
- `TextMeshProUGUI txtName`
- `TextMeshProUGUI txtPrice`
- `TextMeshProUGUI txtXpToUnlock`
- `GameObject unlockButton`

**Methods** (incl. non-public — Harmony hook points)

- `Void Awake()`
- `Void ButtonBuyItem()`
- `Void BuyItem()`
- `Void OnDestroy()`
- `Void OnLoad()`
- `Void Start()`
- `Void TryUnlock()`
- `Void UnlockButton()`
- `Void UpdateVisualState()`

### `ShopItemSO` (class)
*: ScriptableObject*

**Data members** *(read/write state)*

- `Single eol`
- `Boolean isCustomColor`
- `Int32 itemID`
- `String itemName`
- `ObjectInHand itemType`
- `Int32 price`
- `Sprite sprite`
- `Int32 xpToUnlock`

### `ShopItemConfig` (class)

**Data members** *(read/write state)*

- `Il2CppStructArray<Single> colliderCenter`
- `Il2CppStructArray<Single> colliderSize`
- `String iconFile`
- `String itemName`
- `Single mass`
- `String modelFile`
- `Single modelScale`
- `ObjectInHand objectType`
- `Int32 price`
- `Int32 sizeInU`
- `String textureFile`
- `Int32 xpToUnlock`

### `ShopCartItem` (class)
*: MonoBehaviour*

**Data members** *(read/write state)*

- `ButtonExtended btnAdd`
- `ButtonExtended btnRemove`
- `Boolean hasCustomColor`
- `Color itemColor`
- `Int32 itemID`
- `Int32 ItemID` *(read-only)*
- `String itemName`
- `ObjectInHand itemType`
- `ObjectInHand ItemType` *(read-only)*
- `Int32 price`
- `Int32 Quantity` *(read-only)*
- `ButtonExtended RemoveButton` *(read-only)*
- `ComputerShop shop`
- `List<Int32> spawnedItemUIDs`
- `Int32 TotalPrice` *(read-only)*
- `TextMeshProUGUI txtAmount`
- `TextMeshProUGUI txtItemName`
- `TextMeshProUGUI txtPrice`

**Methods** (incl. non-public — Harmony hook points)

- `Void AddSpawnedItem(Int32 uid)`
- `Void ClearAllUIDs()`
- `Void Initialize(ComputerShop shop, String itemName, Int32 itemID, Int32 price, ObjectInHand itemType, Int32 firstSpawnUID, Nullable<Color> customColor)`
- `Void OnAddClicked()`
- `Void OnDestroy()`
- `Void OnRemoveClicked()`
- `Int32 RemoveLastSpawnedItem()`
- `Void UpdateDisplay()`

### `ModShopItem` (class)
*: MonoBehaviour*

**Data members** *(read/write state)*

- `ShopItemConfig config`
- `Image itemIcon`
- `Int32 modID`
- `TextMeshProUGUI txtName`
- `TextMeshProUGUI txtPrice`

**Methods** (incl. non-public — Harmony hook points)

- `Void ButtonBuyItem()`
- `Void Initialize(Int32 modID, ShopItemConfig config, Sprite icon)`

## UI & Localisation

### `StaticUIElements` (class)
*: MonoBehaviour*

**Data members** *(read/write state)*

- `Image blackOver`
- `GameObject canvasStatic`
- `GameObject errorSignPrefab`
- `GameObject explicitSelectOnClose`
- `GameObject goInputNumpadOverlay`
- `GameObject goInputTextOverlay`
- `Image holdKeyIndicator`
- `GameObject imagePointer`
- `Sprite infoSprite`
- `List<PositionIndicator> initiatedErrorWarningSigns`
- `TMP_InputField inputTextOverlayField`
- `StaticUIElements instance`
- `List<GameObject> instantiatedKeyHint`
- `Boolean isInputTextOverlayOpenedFromWorld`
- `Boolean isKeyboardOrMouse`
- `Transform keyboardParent`
- `GameObject keyhint_buttonE`
- `GameObject keyHintCustomPrefab`
- `GameObject loading`
- `Int32 MAX_MESSAGES`
- `Single MESSAGE_DURATION`
- `List<Single> messageExpireTimes`
- `Queue<String> messageQueue`
- `Single moneyMultiplyerPerSecond`
- `Int32 nextErrorWarningUID`
- `Image nextToPointer`
- `GameObject notificationGO`
- `Image notificationSprite`
- `TextMeshProUGUI notificationText`
- `Action<String> onInputNumpadOverlayConfirmed`
- `Func<String, Boolean> onInputNumpadOverlayValidator`
- `Action<String> onInputTextOverlayConfirmed`
- `GameObject prefabParticleUpgrade`
- `GameObject previouslySelectedGameObject`
- `GamepadIcons ps4Icons`
- `Sprite sprite_blackSquare`
- `Sprite sprite_KeyBorder`
- `Sprite sprite_LMB`
- `Sprite sprite_MMB`
- `Sprite sprite_Port_InUse`
- `Sprite sprite_Port_PlugIn`
- `Sprite sprite_Port_Unplug`
- `Sprite sprite_PowerButtonOff`
- `Sprite sprite_PowerButtonOn`
- `Sprite sprite_RMB`
- `Tooltip tooltip`
- `TextMeshProUGUI topLeft_brokenDevices`
- `GameObject topLeft_coinsPrestige`
- `TextMeshProUGUI topLeft_coinTXT`
- `TextMeshProUGUI topLeft_ExpensesPerSecond`
- `TextMeshProUGUI topLeft_MoneyPerSecond`
- `TextMeshProUGUI topLeft_reputationTXT`
- `TextMeshProUGUI topLeft_XPPerSecond`
- `TextMeshProUGUI topLeft_xpTXT`
- `TextMeshProUGUI txtInputTextOverlayTitle`
- `TextMeshProUGUI txtLoadingInfo`
- `TextMeshProUGUI txtMessagesField`
- `TextMeshProUGUI txtNumpadOverlay`
- `TextMeshProUGUI txtUnderPointer`
- `UI_SelectedBorder uI_SelectedBorder`
- `GameObject warningSignPrefab`
- `GamepadIcons xboxIcons`
- `Single xpMultiplyerPerSecond`

**Methods** (incl. non-public — Harmony hook points)

- `Void AddMeesageInField(String message)`
- `Void Awake()`
- `Void ButtonCancelInputNumpadOverlay()`
- `Void ButtonCancelInputTextOverlay()`
- `Void ButtonSaveInputNumpadOverlay()`
- `Void ButtonSaveInputTextOverlay()`
- `Void CalculateRates(ref Single moneyPerSec, ref Single xpPerSec, ref Single expensesPerSec)`
- `Void ClearSpriteNextToPointer()`
- `GameObject CreateCustomKeyHint(InputAction action, Int32 textUID, Transform parent, Boolean isPermanent)`
- `Void DestroyErrorWarningSign(Int32 errorWarningUID)`
- `Void HideTextUnderCursor()`
- `Int32 InstantiateErrorWarningSign(Boolean isError, Vector3 objectPos)`
- `Void InstantiateParticleUpgrade(Transform _transform)`
- `Void OnLoadingStarted()`
- `Void RemoveCustomKeyHint()`
- `Void RestorePreviousSelection()`
- `Void SetLoadingInfo(String s)`
- `Void SetNotification(Int32 _localisationUID, Sprite _sprite, String _text)`
- `Void ShowInputNumpadtOverlay(String title, Action<String> onConfirmed, Func<String, Boolean> validator, GameObject selectOnClose)`
- `Void ShowInputTextOverlay(String title, Action<String> onConfirmed, String defaultText, Boolean isOpenedFromWorld, GameObject selectOnClose)`
- `Void ShowSpriteNextToPointer(Sprite _sprite)`
- `Void ShowStaticCanvas(Boolean active)`
- `Void ShowTextUnderCursor(String text)`
- `Void Start()`
- `IEnumerator UpdateCoinsAndPrestige_TopLeft()`
- `Void UpdateHoldProgress(Single value)`
- `Void UpdateMessageDisplay()`
- `IEnumerator UpdateMessagesCoroutine()`

### `Localisation` (class)
*: MonoBehaviour*

**Data members** *(read/write state)*

- `Languages currentlySelectedLanguage`
- `Dictionary<Int32, String> dictionary`
- `Localisation instance`
- `List<LanguageObject> languages`
- `Int32 loadLanguageUID`
- `OnLanguageChange onLanguageChangedCallback`

**Methods** (incl. non-public — Harmony hook points)

- `Void Awake()`
- `Void ChangeLocalisation(Int32 _uid)`
- `Dictionary<Int32, String> LoadLocalisation(Int32 _uid)`
- `String ReturnTextByID(Int32 _uid)`

### `UI_Section` (class)
*: MonoBehaviour*

**Data members** *(read/write state)*

- `Boolean isOpened`
- `RectTransform rect`
- `GameObject section`

**Methods** (incl. non-public — Harmony hook points)

- `Void OpenCloseSection()`

## Save System

### `SaveSystem` (class)

**Data members** *(read/write state)*

- `Dictionary<String, String> displayToRawMap`
- `Boolean isQuitting`
- `List<String> listofsaves`
- `String loadSaveName`
- `OnLoadingData onLoadingData`
- `OnLoadingDataLater onLoadingDataLater`
- `OnSavingData onSavingData`
- `String saveDirPath`
- `Int32 version`
- `Int32 versionToIgnoreFrom`

**Methods** (incl. non-public — Harmony hook points)

- `static Void AutoSave()`
- `static Void DeleteSaveFile(String savename)`
- `static String FormatDisplayName(String rawEntry)`
- `static BinaryFormatter GetBinaryFormatter()`
- `static String GetRawSaveEntry(String displayName)`
- `static List<String> Listofsaves()`
- `static Void Load(String savename, Boolean isFromPauseMenu)`
- `static SaveData LoadGame(String savename)`
- `static Void LoadGameData()`
- `static String NewestSave()`
- `static SaveMeta ReadMeta(String savename)`
- `static Void SaveGame(String savename, String stringNameOfSave)`
- `static Void SaveGameData()`
- `static Void WriteMeta(String savename, Int32 version, String nameOfSave)`

### `SaveData` (class)

**Data members** *(read/write state)*

- `SaveData _current`
- `List<Vector3> assignedAppsLogos`
- `BalanceSheetSaveData balanceSheetData`
- `Boolean commandCenterAutoClearWarnings`
- `Int32 commandCenterAutoRepairMode`
- `Int32 commandCenterLevel`
- `Il2CppStructArray<Int32> hiredTechnicians`
- `SaveData instance`
- `List<InteractObjectData> interactObjectData`
- `Boolean isIPHintHidden`
- `Il2CppStructArray<Int32> isWallOpened`
- `Int32 lastUsedRackPositionGlobalUID`
- `Il2CppStructArray<Int32> loadedScenes`
- `List<ModItemSaveData> modItemData`
- `String nameOfSave`
- `NetworkSaveData networkData`
- `PlayerData playerData`
- `List<InteractObjectData> rackMountObjectData`
- `List<RepairJobSaveData> repairJobQueue`
- `Boolean saveComplete`
- `ServiceRequestsSaveData serviceRequestsData`
- `Dictionary<String, Boolean> shopItemUnlockStates`
- `List<TechnicianSaveData> technicianData`
- `Vector3 trolleyPosition`
- `Quaternion trolleyRotation`
- `Int32 version`
- `Single wallPrice`

**Methods** (incl. non-public — Harmony hook points)

- `String Validate()`

## Enums

### `CableLink.TypeOfLink` (enum)

| value | name |
|---|---|
| 0 | `None` |
| 1 | `Server` |
| 2 | `Switch` |
| 3 | `Base` |
| 4 | `LB` |
| 5 | `PatchPanel` |

### `FCP_Persistence.SaveStrategy` (enum)

| value | name |
|---|---|
| 0 | `SessionOnly` |
| 1 | `File` |
| 2 | `PlayerPrefs` |

### `FCP_SpriteMeshEditor.MeshType` (enum)

| value | name |
|---|---|
| 0 | `CenterPoint` |
| 1 | `forward` |
| 2 | `backward` |

### `Firewall.Protocol` (enum)

| value | name |
|---|---|
| 0 | `TCP` |
| 1 | `UDP` |
| 2 | `Both` |

### `FlexibleColorPicker.MainPickingMode` (enum)

| value | name |
|---|---|
| 0 | `HS` |
| 1 | `HV` |
| 2 | `SH` |
| 3 | `SV` |
| 4 | `VH` |
| 5 | `VS` |

### `FlexibleColorPicker.PickerType` (enum)

| value | name |
|---|---|
| 0 | `Main` |
| 1 | `R` |
| 2 | `G` |
| 3 | `B` |
| 4 | `H` |
| 5 | `S` |
| 6 | `V` |
| 7 | `A` |
| 8 | `Preview` |
| 9 | `PreviewAlpha` |

### `LeanTweenUIElement.LeanTweanType` (enum)

| value | name |
|---|---|
| 0 | `Horizontal` |
| 1 | `Vertical` |
| 2 | `Scale` |
| 3 | `Rotate` |

### `Localisation.Languages` (enum)

| value | name |
|---|---|
| 1 | `English` |
| 2 | `Czech` |
| 3 | `French` |
| 4 | `Italian` |
| 5 | `German` |
| 6 | `Spanish_Spain` |
| 7 | `Arabic` |
| 8 | `Dutch` |
| 9 | `Japanese` |
| 10 | `Korean` |
| 11 | `Portuguese_Brazil` |
| 12 | `Portuguese_Portugal` |
| 13 | `Russian` |
| 14 | `Simplified_Chinese` |
| 15 | `Spanish_LatinAmerica` |
| 16 | `Turkish` |
| 17 | `Polish` |
| 18 | `Thai` |
| 19 | `Traditional_Chinese` |

### `PlayerManager.ObjectInHand` (enum)

| value | name |
|---|---|
| 0 | `None` |
| 1 | `Server1U` |
| 2 | `Server2U` |
| 3 | `Server3U` |
| 4 | `Switch` |
| 5 | `Rack` |
| 6 | `CableSpinner` |
| 7 | `PatchPanel` |
| 8 | `SFPModule` |
| 9 | `SFPBox` |
| 10 | `ModItem` |
| 11 | `Router` |
| 12 | `Firewall` |

### `ServiceRequest.SRState` (enum)

| value | name |
|---|---|
| 0 | `InProgress` |
| 1 | `Resolved` |

### `Technician.TechnicianState` (enum)

| value | name |
|---|---|
| 0 | `Idle` |
| 1 | `GoingForNewServer` |
| 2 | `BringingNewServer` |
| 3 | `GoingBackWithOldServer` |
| 4 | `EndingHisWork` |

### `UserReport.UserReportingState` (enum)

| value | name |
|---|---|
| 0 | `Idle` |
| 1 | `CreatingUserReport` |
| 2 | `ShowingForm` |
| 3 | `SubmittingForm` |

## Save Data Structures

### `BalanceSheetSaveData` (class)

**Data members** *(read/write state)*

- `List<CustomerRecordSaveData> currentRecords`
- `Single currentRepairExpense`
- `Single currentSalaryExpense`
- `Single currentShopExpense`
- `List<MonthlySnapshotSaveData> history`
- `Single totalMonthlySalary`

### `CableEndpointSaveData` (class)

**Data members** *(read/write state)*

- `Int32 customerID`
- `Vector3 position`
- `String serverID`
- `String switchID`
- `TypeOfLink type`

### `CableSaveData` (class)

**Data members** *(read/write state)*

- `Color cableColor`
- `Int32 cableID`
- `CableEndpointSaveData endPoint`
- `Single maxSpeed`
- `List<Vector3> midPointPositions`
- `CableEndpointSaveData startPoint`
- `List<Vector3> waypoints`

### `CustomerBaseSaveData` (class)

**Data members** *(read/write state)*

- `Dictionary<Int32, Int32> appObjectiveIDs`
- `Il2CppStructArray<Boolean> appReputationAwarded`
- `Il2CppStructArray<Single> appsSpeedRequirements`
- `Il2CppStructArray<Int32> appsTimeBelowRequirements`
- `Il2CppStructArray<Int32> appTypes`
- `Int32 customerBaseID`
- `Int32 customerID`
- `Int32 difficulty`
- `Dictionary<Int32, String> subnetsPerApp`
- `Dictionary<Int32, Int32> vlanIdsPerApp`

### `CustomerRecordSaveData` (class)

**Data members** *(read/write state)*

- `Int32 customerID`
- `String customerName`
- `Single penalties`
- `Single revenue`

### `FirewallSaveData` (class)
*: SwitchSaveData*

**Data members** *(read/write state)*

- `String clusterIP`
- `List<FilterRule> filterRules`

### `LACPGroupSaveData` (class)

**Data members** *(read/write state)*

- `List<Int32> cableIds`
- `String deviceA`
- `String deviceB`
- `Int32 groupId`

### `ModItemSaveData` (class)

**Data members** *(read/write state)*

- `String modFolderName`
- `Vector3 position`
- `Quaternion rotation`
- `Il2CppStructArray<Int32> saveIntArray`
- `Il2CppStructArray<Int32> saveIntArray2`
- `Il2CppStructArray<Single> saveValue`

### `MonthlySnapshotSaveData` (class)

**Data members** *(read/write state)*

- `Int32 day`
- `Int32 month`
- `List<CustomerRecordSaveData> records`
- `Single repairExpense`
- `Single salaryExpense`
- `Single shopExpense`

### `NetworkSaveData` (class)

**Data members** *(read/write state)*

- `List<CableLinkLabelData> cableLinkLabels`
- `List<CableSaveData> cables`
- `List<CustomerBaseSaveData> customerBases`
- `List<FirewallSaveData> firewalls`
- `List<LACPGroupSaveData> lacpGroups`
- `List<PatchPanelSaveData> patchPanels`
- `List<RouterSaveData> routers`
- `List<ServerSaveData> servers`
- `List<SFPSaveData> sfpModules`
- `List<SwitchSaveData> switches`

### `PatchPanelSaveData` (class)

**Data members** *(read/write state)*

- `String patchPanelID`
- `Int32 patchPanelType`
- `Vector3 position`
- `Int32 rackPositionUID`
- `Quaternion rotation`

### `RepairJobSaveData` (class)

**Data members** *(read/write state)*

- `String serverID`
- `String switchID`

### `RouterSaveData` (class)
*: SwitchSaveData*

**Data members** *(read/write state)*

- `Int32 asn`
- `List<SubnetRoute> routingTable`

### `SaveData` (class)

**Data members** *(read/write state)*

- `SaveData _current`
- `List<Vector3> assignedAppsLogos`
- `BalanceSheetSaveData balanceSheetData`
- `Boolean commandCenterAutoClearWarnings`
- `Int32 commandCenterAutoRepairMode`
- `Int32 commandCenterLevel`
- `Il2CppStructArray<Int32> hiredTechnicians`
- `SaveData instance`
- `List<InteractObjectData> interactObjectData`
- `Boolean isIPHintHidden`
- `Il2CppStructArray<Int32> isWallOpened`
- `Int32 lastUsedRackPositionGlobalUID`
- `Il2CppStructArray<Int32> loadedScenes`
- `List<ModItemSaveData> modItemData`
- `String nameOfSave`
- `NetworkSaveData networkData`
- `PlayerData playerData`
- `List<InteractObjectData> rackMountObjectData`
- `List<RepairJobSaveData> repairJobQueue`
- `Boolean saveComplete`
- `ServiceRequestsSaveData serviceRequestsData`
- `Dictionary<String, Boolean> shopItemUnlockStates`
- `List<TechnicianSaveData> technicianData`
- `Vector3 trolleyPosition`
- `Quaternion trolleyRotation`
- `Int32 version`
- `Single wallPrice`

**Methods**

- `String Validate()`

### `ServerSaveData` (class)

**Data members** *(read/write state)*

- `Int32 customerID`
- `Int32 eolTime`
- `String ip`
- `Boolean isBroken`
- `Boolean isOn`
- `Boolean isWarningCleared`
- `String label`
- `Vector3 position`
- `Int32 prefabID`
- `Int32 rackPositionUID`
- `Quaternion rotation`
- `String serverID`
- `Int32 serverType`
- `Int32 timeToBrake`

### `ServiceRequestsSaveData` (class)

**Data members** *(read/write state)*

- `Int32 currentSRNumber`
- `List<ServiceRequest> requests`

### `SFPSaveData` (class)

**Data members** *(read/write state)*

- `Boolean isInserted`
- `Vector3 portPosition`
- `Vector3 position`
- `Int32 prefabID`
- `Quaternion rotation`

### `SwitchSaveData` (class)

**Data members** *(read/write state)*

- `Int32 eolTime`
- `Boolean isBroken`
- `Boolean isOn`
- `Boolean isWarningCleared`
- `String label`
- `List<PortVlanFilterData> portVlanFilters`
- `Vector3 position`
- `Int32 rackPositionUID`
- `Quaternion rotation`
- `String switchID`
- `Int32 switchType`
- `Int32 timeToBrake`

### `TechnicianSaveData` (class)

**Data members** *(read/write state)*

- `Il2CppStructArray<Single> position`
- `Int32 technicianID`

## Full Game Type Catalog (remaining)

| type | kind | fields | methods |
|---|---|---|---|
| `_PrivateImplementationDetails_` | class | 10 | 3 |
| `ActionKeyHint` | class | 19 | 10 |
| `AICharacterControl` | class | 39 | 16 |
| `AICharacterExpressions` | class | 16 | 15 |
| `AssetManagement` | class | 39 | 20 |
| `AssetManagementDeviceLine` | class | 16 | 6 |
| `AssetManagementDeviceLineData` | class | 11 | 4 |
| `AutoDisable` | class | 6 | 5 |
| `AutoScrollRect` | class | 10 | 6 |
| `BalanceSheet` | class | 37 | 25 |
| `BalanceSheetRow` | class | 20 | 9 |
| `CableIDComponent` | struct | 4 | 2 |
| `CableLinkLabelData` | class | 3 | 3 |
| `CablePositions` | class | 50 | 23 |
| `CableSpinner` | class | 16 | 12 |
| `CarryModelPool` | class | 10 | 9 |
| `ChatController` | class | 7 | 6 |
| `CheckIfTouchingWall` | class | 9 | 9 |
| `ColorSerializationSurrogate` | class | 3 | 5 |
| `CommandCenter` | class | 26 | 14 |
| `CommandCenterOperator` | class | 4 | 6 |
| `CreateSubnetAndMoveServersSR` | class | 6 | 5 |
| `CustomerBase` | class | 63 | 28 |
| `CustomerBaseDoor` | class | 13 | 11 |
| `CustomerCard` | class | 13 | 4 |
| `CustomerColor` | class | 2 | 4 |
| `CustomerItem` | class | 9 | 3 |
| `DllEntry` | class | 3 | 3 |
| `DropdownSample` | class | 5 | 4 |
| `Dumpster` | class | 9 | 7 |
| `EnvMapAnimator` | class | 6 | 5 |
| `FCP_Persistence` | class | 18 | 13 |
| `FCP_SpriteMeshEditor` | class | 9 | 6 |
| `FlexibleColorPicker` | class | 87 | 57 |
| `FootSteps` | class | 23 | 10 |
| `GamepadIcons` | class | 21 | 4 |
| `GateLever` | class | 22 | 11 |
| `GetCurrentVersion` | class | 2 | 4 |
| `GetValueFromPlayerPrefs` | class | 3 | 4 |
| `GODMOD` | class | 12 | 9 |
| `HRSystem` | class | 14 | 9 |
| `IModPlugin` | class | 2 | 4 |
| `InputController` | class | 58 | 12 |
| `Interact` | class | 24 | 12 |
| `InteractObjectData` | class | 8 | 3 |
| `Internet` | class | 17 | 5 |
| `Item` | class | 11 | 3 |
| `KeyHint` | class | 5 | 5 |
| `LanguageObject` | class | 4 | 3 |
| `LeanTweenUIElement` | class | 27 | 14 |
| `LoadingScreen` | class | 20 | 14 |
| `LocalisedText` | class | 6 | 7 |
| `MainMenu` | class | 15 | 11 |
| `MainMenuCamera` | class | 9 | 6 |
| `ModLoader` | class | 28 | 22 |
| `ModPackConfig` | class | 5 | 3 |
| `MusicPlayer` | class | 11 | 9 |
| `Numpad` | class | 14 | 12 |
| `ObjectiveObject` | class | 8 | 6 |
| `Objectives` | class | 37 | 20 |
| `ObjectiveTimed` | class | 10 | 5 |
| `ObjectPrivateAbstractSealedInVo0` | class | 1 | 3 |
| `ObjImporter` | class | 3 | 5 |
| `OpenURL` | class | 3 | 4 |
| `PacketComponent` | struct | 16 | 2 |
| `PacketSettings` | struct | 2 | 2 |
| `PacketSpawnerAuthoring` | class | 4 | 3 |
| `PacketSpawnerComponent` | class | 13 | 3 |
| `PacketSpawnerSystem` | class | 17 | 9 |
| `PatchPanel` | class | 13 | 12 |
| `PauseMenu` | class | 47 | 24 |
| `PauseMenu_TabButton` | class | 11 | 8 |
| `PauseMenu_TabGroup` | class | 9 | 8 |
| `PauseMenuVideoTutorial` | class | 2 | 4 |
| `PortVlanFilterData` | class | 3 | 3 |
| `PositionIndicator` | class | 6 | 5 |
| `PulsatingImageColor` | class | 14 | 9 |
| `PulsatingText` | class | 11 | 7 |
| `PushTrolleyHandle` | class | 7 | 7 |
| `QuaternionSerializationSurrogate` | class | 3 | 5 |
| `RackAudioCuller` | class | 16 | 9 |
| `ReBindUI` | class | 24 | 11 |
| `RebindUIv2` | class | 40 | 12 |
| `RectExtensions` | class | 2 | 4 |
| `ReusableFunctions` | class | 11 | 13 |
| `ServiceRequest` | class | 7 | 5 |
| `ServiceRequestRow` | class | 9 | 5 |
| `ServiceRequests` | class | 18 | 14 |
| `SetIP` | class | 34 | 25 |
| `SettingsControls` | class | 9 | 7 |
| `SettingsGameplay` | class | 17 | 11 |
| `SettingsGraphics` | class | 50 | 23 |
| `SettingsSingleton` | class | 6 | 5 |
| `SettingsVolume` | class | 13 | 9 |
| `SFPBox` | class | 15 | 14 |
| `SFPModule` | class | 17 | 12 |
| `StaticItemConfig` | class | 10 | 3 |
| `SteamLeaderboards` | class | 18 | 8 |
| `SteamStatsOnMainMenuTop` | class | 9 | 7 |
| `Technician` | class | 50 | 22 |
| `TerrainDetector` | class | 10 | 6 |
| `Tooltip` | class | 12 | 6 |
| `ToolTipInteract` | class | 6 | 5 |
| `ToolTipOnUIText` | class | 11 | 10 |
| `TrolleyLoadingBay` | class | 16 | 11 |
| `TrolleyTrigger` | class | 5 | 5 |
| `Tutorials` | class | 22 | 14 |
| `UI_SelectedBorder` | class | 6 | 6 |
| `UIExtension` | class | 5 | 7 |
| `UnitySourceGeneratedAssemblyMonoScriptTypes_v1` | class | 2 | 4 |
| `UsableObject` | class | 67 | 22 |
| `UsableObjectPhysicsUpdater` | class | 4 | 5 |
| `UserReport` | class | 25 | 11 |
| `Vector3SerializationSurrogate` | class | 3 | 5 |
| `Wall` | class | 12 | 10 |
| `WaypointInitializationSystem` | class | 63 | 46 |
| `WaypointsBlob` | class | 1 | 3 |
| `WorldCanvasCuller` | class | 12 | 7 |
| `WorldObjectButton` | class | 7 | 7 |
