import { INDEX_CRUMB } from '@/breadcrumbs/general';
import { APPOINTMENTS_CRUMB } from '@/breadcrumbs/appointments';
import { PRESCRIPTIONS_CRUMB } from '@/breadcrumbs/prescriptions';

const GP_SESSION_ON_DEMAND_CRUMB = {
  defaultCrumb: [INDEX_CRUMB],
  onDemandAppointmentCrumb: [INDEX_CRUMB, APPOINTMENTS_CRUMB],
  onDemandPrescriptionCrumb: [INDEX_CRUMB, PRESCRIPTIONS_CRUMB],
  onDemandLinkedProfiles: [INDEX_CRUMB],
};

export default {
  GP_SESSION_ON_DEMAND_CRUMB,
};
