//Documentation for the step plugin

/*
public class StepCounterPlugin implements SensorEventListener {

    private static Context appContext;
    private static SensorManager sensorManager;
    private static Sensor stepSensor;

    private static int steps = 0;
    private static int initialSteps = 0;

    public static void init(Context context) {
        appContext = context;
        sensorManager = (SensorManager) context.getSystemService(Activity.SENSOR_SERVICE);
        stepSensor = sensorManager.getDefaultSensor(Sensor.TYPE_STEP_COUNTER);
    }

    private static StepCounterPlugin instance;

    public static StepCounterPlugin getInstance() {
        if (instance == null) {
            instance = new StepCounterPlugin();
        }
        return instance;
    }

    public static void startCounting() {
        // Check the current step count when starting to count
        initialSteps = steps;
        sensorManager.registerListener(getInstance(), stepSensor, SensorManager.SENSOR_DELAY_UI);
    }

    public static int getStepsSinceStart() {
        return steps - initialSteps;
    }

    public static void stopCounting() {
        sensorManager.unregisterListener(getInstance());
    }

    public static int getSteps() {
        return steps;
    }

    @Override
    public void onSensorChanged(SensorEvent sensorEvent) {
        if (sensorEvent.sensor.getType() == Sensor.TYPE_STEP_COUNTER) {
            steps = (int) sensorEvent.values[0];
        }
    }

    @Override
    public void onAccuracyChanged(Sensor sensor, int i) {
    }
}





*/