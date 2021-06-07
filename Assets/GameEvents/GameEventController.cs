using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventController : MonoBehaviour
{
    [SerializeField] private TimeController timeController;
    [SerializeField] private RailController railController;
    [SerializeField] private TrainController trainController;
    [SerializeField] private ResourceController resourceController;
    [SerializeField] public GameEventImages eventImages;

    private List<List<GameEvent>> events;

    private float nextEventInSeconds;

    public int indentation = 8;

    public GameEvent guarranteedNextEvent = null;

    private void Awake() {
        setNextEventDate();
        eventImages = GetComponent<GameEventImages>();
        events = new List<List<GameEvent>>();
    }

    private void Start() {
        StartCoroutine(waitUntilDirectConnectionsExist());
    }

    private IEnumerator waitUntilDirectConnectionsExist() {
        yield return new WaitUntil(() => railController.directConnections != null && railController.directConnections.Count > 0);
        events.Add(createTrainDisasterEvents());
        events.Add(createAGoodPeriodEvents());
        events.Add(createCometSightedEvents());
        events.Add(createCrewStrikeEvents());
    }

    private List<GameEvent> createCrewStrikeEvents() {
        TrainTypes[] professions = new TrainTypes[] { TrainTypes.CARGO, TrainTypes.PASSENGER };
        List<GameEvent> crewStrikeEvents = new List<GameEvent>();

        foreach (TrainTypes profession in professions) {
            //Option 1
            GameEventOption eventOption1 = new GameEventOption("Give in to their demands",
             $"Maintenance costs for {profession.ToString().ToLower()} trains will be <color=purple>tripled (3x)</color> for two days");
            MaintenanceCostEffect maintenanceCostEffect = new MaintenanceCostEffect(trainController, 3, 190, profession);
            KeyValuePair<GameEventOption, GameEventEffect[]> o1 = new KeyValuePair<GameEventOption, GameEventEffect[]>(eventOption1, new GameEventEffect[] { maintenanceCostEffect });

            //Option 2
            GameEventOption eventOption2 = new GameEventOption("Let them publish the report",
             $"Half of all deals will be cancelled");
            DealRemoveEffect dealRemoveEffect = new DealRemoveEffect(resourceController, true);
            KeyValuePair<GameEventOption, GameEventEffect[]> o2 = new KeyValuePair<GameEventOption, GameEventEffect[]>(eventOption2, new GameEventEffect[] { dealRemoveEffect });

            GameEvent gameEvent = new GameEvent("Strikes!", $"We want living wages! We want living wages! We want living wages!\n" + 
                $"This was the consensus of strikers today. People working in our {profession.ToString().ToLower()} trains are demanding better wages " +
                $"whilst claiming to have a detailed report on poor working conditions. They say if we refuse their demands, then they will give it to the press.",
            GameEventTypes.ECONOMY, o1, o2);

            crewStrikeEvents.Add(gameEvent);
        }

        return crewStrikeEvents;
    }

    private List<GameEvent> createStockMarketCrashEvents() {
        List<GameEvent> stockMarketCrashEvents = new List<GameEvent>();

        //Option 1
        StockMarketEffect marketEffectBad = new StockMarketEffect(resourceController, -10);
        GameEventOption eventOption1 = new GameEventOption("Damn comets!",
              $"All prices on goods will be reduced to 1 kr");
        KeyValuePair<GameEventOption, GameEventEffect[]> o1 = new KeyValuePair<GameEventOption, GameEventEffect[]>(eventOption1, new GameEventEffect[] { marketEffectBad });

        //Option 2
        StockMarketEffect marketEffectSmall = new StockMarketEffect(resourceController, -2);
        int cost = -10000;
        MoneyEffect moneyEffect = new MoneyEffect(cost);
        GameEventOption eventOption2 = new GameEventOption("Oh, you can't do this to me",
              $"1.<indent={indentation}%>All prices on goods will be reduced by -2 kr.</indent>\n" +
              $"2.<indent={indentation}%>You will lose {cost} kr</indent>\n");
        KeyValuePair<GameEventOption, GameEventEffect[]> o2 = new KeyValuePair<GameEventOption, GameEventEffect[]>(eventOption2, new GameEventEffect[] { marketEffectSmall, moneyEffect });


        GameEvent gameEvent = new GameEvent("Stock Market Crash!", $"The stock market crashed today as a result of the sighted comet. " +
            $"The bad omen has led to all stocks to being worthless due to investors looking to mitigate their losses. " +
            $"What a tragedy!",
            GameEventTypes.ECONOMY, o1, o2);

        stockMarketCrashEvents.Add(gameEvent);

        return stockMarketCrashEvents;
    }

    private List<GameEvent> createCometSightedEvents() {
        List<GameEvent> cometEvents = new List<GameEvent>();

        GuaranteedNextEventEffect nextEventEffect = new GuaranteedNextEventEffect(createStockMarketCrashEvents()[0], this);

        GameEventOption eventOption1 = new GameEventOption("It's an omen",
              $"You sense problems looming...");
        KeyValuePair<GameEventOption, GameEventEffect[]> o1 = new KeyValuePair<GameEventOption, GameEventEffect[]>(eventOption1, new GameEventEffect[] { nextEventEffect });

        GameEventOption eventOption2 = new GameEventOption("The end is nigh!",
              $"You sense problems looming...");
        KeyValuePair<GameEventOption, GameEventEffect[]> o2 = new KeyValuePair<GameEventOption, GameEventEffect[]>(eventOption2, new GameEventEffect[] { nextEventEffect });

        GameEventOption eventOption3 = new GameEventOption("The economy, fools",
              $"You sense problems looming...");
        KeyValuePair<GameEventOption, GameEventEffect[]> o3 = new KeyValuePair<GameEventOption, GameEventEffect[]>(eventOption3, new GameEventEffect[] { nextEventEffect });

        GameEventOption eventOption4 = new GameEventOption("I wish I lived in more enlightened times...",
              $"You sense problems looming...");
        KeyValuePair<GameEventOption, GameEventEffect[]> o4 = new KeyValuePair<GameEventOption, GameEventEffect[]>(eventOption4, new GameEventEffect[] { nextEventEffect });


        GameEvent gameEvent = new GameEvent("Comet Sighted", $"Venture capitalists are always superstitious, " +
            $"and the appearance of a comet in the sky has caused panic among dealmakers. " +
            $"They are convinced that this is a sign that the end of times is near or that something bad is going to happend in the future.",
            GameEventTypes.COMET, o1, o2, o3, o4);

        cometEvents.Add(gameEvent);

        return cometEvents;
    }

    private List<GameEvent> createAGoodPeriodEvents() {
        List<GameEvent> goodPeriodEvents = new List<GameEvent>();

        int profit = 4000;
        GameEventOption eventOption1 = new GameEventOption("This will be reflected in our profit margins.",
              $"1.<indent={indentation}%>You will gain {profit} kr.</indent>");
        MoneyEffect o1e1 = new MoneyEffect(profit);
        KeyValuePair<GameEventOption, GameEventEffect[]> o1 = new KeyValuePair<GameEventOption, GameEventEffect[]>(eventOption1, new GameEventEffect[] { o1e1 });

        GameEventOption eventOption2 = new GameEventOption("Spend it on the rails!",
            $"1.<indent={indentation}%>Each rail will recieve +2 health</indent>");
        RailHurtEffect o2e1 = new RailHurtEffect(railController, -2);
        KeyValuePair<GameEventOption, GameEventEffect[]> o2 = new KeyValuePair<GameEventOption, GameEventEffect[]>(eventOption2, new GameEventEffect[] { o2e1 });

        GameEvent gameEvent = new GameEvent("Booming Economy!", $"Train are running on time, trade is bigger than ever, and customers are happy." +
            $" Our current fiscal state gives us a hefty profit margin." +
            $" What should we do with it?", GameEventTypes.ECONOMY, o1, o2);

        goodPeriodEvents.Add(gameEvent);

        return goodPeriodEvents;
    }

    private List<GameEvent> createTrainDisasterEvents() {
        List<GameEvent> disasterEvents = new List<GameEvent>();

        //For every direct connection, add event with possibility of train disaster
        foreach (Connection c in railController.directConnections.Keys) {

            int cost = -4000;
            string cityName = XMLParser.cityNameMap[c.end];
            string homeCityName = XMLParser.cityNameMap[c.start];
            string randomCity = XMLParser.cityNameMap[Random.Range(1, XMLParser.cityNameMap.Count)];

            GameEventOption eventOption1 = new GameEventOption("Try to hide it, whatever the cost.", 
                $"1.<indent={indentation}%>You will lose {cost} kr.</indent>\n" +           
                $"2.<indent={indentation}%>The rail to {cityName} will lose 8 health.</indent>");
            MoneyEffect o1e1 = new MoneyEffect(cost);
            
            RailHurtEffect o1e3 = new RailHurtEffect(railController, c, 8);
            KeyValuePair<GameEventOption, GameEventEffect[]> o1 = new KeyValuePair<GameEventOption, GameEventEffect[]>(eventOption1, new GameEventEffect[] { o1e1, o1e3 });

            GameEventOption eventOption2 = new GameEventOption("I want this fixed and I want it NOW.", $"1.<indent=5%>The rail to {cityName} will lose 15 health.</indent>\n" +
                $"2.<indent={indentation}%>All deals to {cityName} will be suspended.</indent>\n");
            RailHurtEffect o2e1 = new RailHurtEffect(railController, c, 15);
            DealRemoveEffect o2e2 = new DealRemoveEffect(resourceController, c.end);
            KeyValuePair<GameEventOption, GameEventEffect[]> o2 = new KeyValuePair<GameEventOption, GameEventEffect[]>(eventOption2, new GameEventEffect[] { o2e1, o2e2 });

            GameEvent gameEvent = new GameEvent($"A disaster for {cityName}!", $"Today, " +
                $"a terrible disaster struck the route between {homeCityName} and {cityName}. " +
                $"A large rock had seemingly wedged itself onto the track when an incoming train from {randomCity} approached. \n\n" +
                $"The incoming train ran the rock over and derailed shortly afterwars, leaving the route momentarily blocked and the railroad damaged. " +
                $"Police suspect it to be a deliberate attack and are investigating the matter.", GameEventTypes.DERAIL, o1, o2);

            disasterEvents.Add(gameEvent);
        }
        return disasterEvents;
    }
    private void setNextEventDate() {
        nextEventInSeconds = Random.Range(60f, 240f);
    }

    private void Update() {
        nextEventInSeconds -= Time.deltaTime;
        if(nextEventInSeconds <= 0) {
            setNextEventDate();
            timeController.forcePause();
            initializeRandomEvent();
        }
    }

    private void initializeRandomEvent() {

        GameEvent randomEvent = null;

        //if an event is already predetermined, i.e event chain
        if(guarranteedNextEvent != null) {
            randomEvent = guarranteedNextEvent;
            guarranteedNextEvent = null;
        } else {
            List<GameEvent> randomEvents = events[Random.Range(0, events.Count)];
            randomEvent = randomEvents[Random.Range(0, randomEvents.Count)];
        }
        

        PopupSystem.ShowEventPopup(randomEvent.title, randomEvent.description, eventImages.GetEventImage(randomEvent.type), randomEvent.options, (i) => {
            randomEvent.invokeOption(i);
            PopupSystem.HideEventPopup();
            timeController.forceResume();
        });

        Analytics.IncrementCounter("events_seen");
    }


}
