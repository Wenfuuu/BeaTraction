import http from 'k6/http';
import { check, sleep } from 'k6';
import { Counter, Trend } from 'k6/metrics';

const BASE_URL = __ENV.BASE_URL || 'http://localhost:5271';
const SCHEDULE_ATTRACTION_ID = __ENV.SCHEDULE_ATTRACTION_ID || '3fa85f64-5717-4562-b3fc-2c963f66afa6';
const EXPECTED_CAPACITY = parseInt(__ENV.EXPECTED_CAPACITY || '100');

const successCounter = new Counter('registrations_success');
const failureCounter = new Counter('registrations_failed');
const capacityErrorCounter = new Counter('registrations_capacity_full');
const duplicateErrorCounter = new Counter('registrations_duplicate');
const otherErrorCounter = new Counter('registrations_other_error');
const responseTime = new Trend('registration_response_time');

export const options = {
  scenarios: {
    // Simulates high concurrency - 100 users/sec trying to register simultaneously
    capacity_stress_test: {
      executor: 'constant-arrival-rate',
      rate: 100,              // 100 requests per second
      timeUnit: '1s',
      duration: '30s',        // Run for 30 seconds
      preAllocatedVUs: 1000,  // Pre-allocate 1000 virtual users
      maxVUs: 2000,           // Max 2000 VUs if needed
    },
  },
  
  thresholds: {
    // Performance thresholds
    'http_req_duration': ['p(95)<1500', 'p(99)<2000'], // 95% < 1.5s, 99% < 2s
    'http_req_failed{expected:false}': ['rate<0.01'],  // Less than 1% unexpected failures
    'registrations_success': [`count<=${EXPECTED_CAPACITY + 5}`], // Allow small buffer for race conditions
  },
};

function generateGuid() {
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
    const r = Math.random() * 16 | 0;
    const v = c === 'x' ? r : (r & 0x3 | 0x8);
    return v.toString(16);
  });
}

export default function () {
  const uniqueUserId = generateGuid();
  
  const payload = JSON.stringify({
    userId: uniqueUserId,
    scheduleAttractionId: SCHEDULE_ATTRACTION_ID,
    registeredAt: new Date().toISOString()
  });

  const params = {
    headers: {
      'Content-Type': 'application/json',
    },
    tags: { name: 'CreateRegistration' },
  };

  const startTime = Date.now();
  const response = http.post(
    `${BASE_URL}/api/registrations`,
    payload,
    params
  );
  const duration = Date.now() - startTime;
  
  responseTime.add(duration);

  let responseBody = {};
  try {
    responseBody = JSON.parse(response.body || '{}');
  } catch (e) {
    console.warn(`Failed to parse response: ${response.body}`);
  }

  const isSuccess = response.status === 200 || response.status === 201;
  const isCapacityError = response.status === 400 && 
                          (responseBody.message || '').toLowerCase().includes('capacity');
  const isDuplicateError = response.status === 400 && 
                           (responseBody.message || '').toLowerCase().includes('already registered');
  const isConflict = response.status === 409;

  if (isSuccess) {
    successCounter.add(1);
    console.log(`VU${__VU} - Registration SUCCESS (Total: ${successCounter.value})`);
  } else if (isCapacityError || isConflict) {
    capacityErrorCounter.add(1);
    failureCounter.add(1);
  } else if (isDuplicateError) {
    duplicateErrorCounter.add(1);
    failureCounter.add(1);
  } else {
    otherErrorCounter.add(1);
    failureCounter.add(1);
    console.error(`VU${__VU} - Unexpected error: ${response.status} - ${responseBody.message || response.body}`);
  }

  check(response, {
    'status is 2xx or expected error': (r) => 
      r.status === 200 || r.status === 201 || r.status === 400 || r.status === 409,
    
    'no over-booking (success count <= capacity)': () => 
      successCounter.value <= EXPECTED_CAPACITY,
    
    'appropriate error message when full': (r) => {
      if (r.status === 400 || r.status === 409) {
        const msg = (responseBody.message || '').toLowerCase();
        return msg.includes('capacity') || msg.includes('full') || msg.includes('already');
      }
      return true;
    },
    
    'response time acceptable': () => duration < 3000,
  });

  sleep(0.1);
}

export function setup() {
  console.log('\n========================================');
  console.log('   STARTING STRESS TEST');
  console.log('========================================');
  console.log(`   Target: ${BASE_URL}`);
  console.log(`   Schedule Attraction ID: ${SCHEDULE_ATTRACTION_ID}`);
  console.log(`   Expected Capacity: ${EXPECTED_CAPACITY}`);
  console.log(`   Duration: 30 seconds`);
  console.log(`   Target Rate: 100 requests/second`);
  console.log(`   Max VUs: 2000`);
  console.log('========================================\n');
  
  const healthCheck = http.get(`${BASE_URL}/health`);
  if (healthCheck.status !== 200 && healthCheck.status !== 404) {
    console.error(`   Backend may not be accessible: ${healthCheck.status}`);
  }
  
  return {
    startTime: new Date().toISOString(),
  };
}

export function teardown(data) {
  console.log('\n========================================');
  console.log('  TEST COMPLETED');
  console.log('========================================');
  console.log(`Started: ${data.startTime}`);
  console.log(`Ended: ${new Date().toISOString()}`);
  console.log('========================================\n');
}

// ============================================
// 📄 SUMMARY REPORT
// ============================================

export function handleSummary(data) {
  const successCount = data.metrics.registrations_success?.values?.count || 0;
  const failureCount = data.metrics.registrations_failed?.values?.count || 0;
  const capacityErrors = data.metrics.registrations_capacity_full?.values?.count || 0;
  const duplicateErrors = data.metrics.registrations_duplicate?.values?.count || 0;
  const otherErrors = data.metrics.registrations_other_error?.values?.count || 0;
  const totalRequests = successCount + failureCount;
  
  const avgResponseTime = data.metrics.registration_response_time?.values?.avg || 0;
  const p95ResponseTime = data.metrics.registration_response_time?.values['p(95)'] || 0;
  const p99ResponseTime = data.metrics.registration_response_time?.values['p(99)'] || 0;
  
  const overBooked = successCount > EXPECTED_CAPACITY;
  const exactMatch = successCount === EXPECTED_CAPACITY;
  
  console.log('\n');
  console.log('========================================');
  console.log('   STRESS TEST RESULTS');
  console.log('========================================');
  console.log(`   Successful Registrations: ${successCount}`);
  console.log(`   Total Failures: ${failureCount}`);
  console.log(`   └─ Capacity Full: ${capacityErrors}`);
  console.log(`   └─ Duplicate: ${duplicateErrors}`);
  console.log(`   └─ Other Errors: ${otherErrors}`);
  console.log(`    Expected Capacity: ${EXPECTED_CAPACITY}`);
  console.log(`    Total Requests: ${totalRequests}`);
  console.log('----------------------------------------');
  console.log(`    Avg Response Time: ${avgResponseTime.toFixed(2)}ms`);
  console.log(`    P95 Response Time: ${p95ResponseTime.toFixed(2)}ms`);
  console.log(`    P99 Response Time: ${p99ResponseTime.toFixed(2)}ms`);
  console.log('========================================');
  
  if (overBooked) {
    console.log(`\nCRITICAL FAILURE: Over-booking detected!`);
    console.log(`   Registered: ${successCount}, Capacity: ${EXPECTED_CAPACITY}`);
    console.log(`   Over-booked by: ${successCount - EXPECTED_CAPACITY}`);
    console.log(`   Redis optimistic locking may not be working correctly!`);
  } else if (exactMatch) {
    console.log(`\nSUCCESS: Exactly ${EXPECTED_CAPACITY} users registered!`);
    console.log(`   No over-booking occurred.`);
    console.log(`   Redis optimistic locking is working perfectly!`);
  } else if (successCount < EXPECTED_CAPACITY) {
    console.log(`\nOnly ${successCount} registered out of ${EXPECTED_CAPACITY} capacity`);
    console.log(`   This may be okay if the test duration was too short.`);
    console.log(`   Try increasing duration or rate.`);
  }
  
  console.log('========================================\n');
  
  return {
    'stdout': textSummary(data, { indent: ' ', enableColors: true }),
    'stress-test-results.json': JSON.stringify({
      testConfig: {
        baseUrl: BASE_URL,
        scheduleAttractionId: SCHEDULE_ATTRACTION_ID,
        expectedCapacity: EXPECTED_CAPACITY,
        duration: '30s',
        targetRate: 100,
      },
      results: {
        successCount,
        failureCount,
        capacityErrors,
        duplicateErrors,
        otherErrors,
        totalRequests,
        overBooked,
        exactMatch,
      },
      performance: {
        avgResponseTime: avgResponseTime.toFixed(2),
        p95ResponseTime: p95ResponseTime.toFixed(2),
        p99ResponseTime: p99ResponseTime.toFixed(2),
      },
      verdict: overBooked ? 'FAILED - Over-booking detected' : 
               exactMatch ? 'PASSED - Perfect capacity enforcement' : 
               'PARTIAL - Under-capacity',
      timestamp: new Date().toISOString(),
    }, null, 2),
  };
}

function textSummary(data, options = {}) {
  const indent = options.indent || '';
  let summary = '';
  
  if (data.metrics) {
    summary += `${indent}Metrics:\n`;
    for (const [name, metric] of Object.entries(data.metrics)) {
      if (metric.values) {
        summary += `${indent}  ${name}:\n`;
        for (const [key, value] of Object.entries(metric.values)) {
          summary += `${indent}    ${key}: ${typeof value === 'number' ? value.toFixed(2) : value}\n`;
        }
      }
    }
  }
  
  return summary;
}
