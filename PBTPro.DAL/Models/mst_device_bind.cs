using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// This table stores information about devices bound to the PBT Pro system. When a user logs into the application, it captures details such as the unique device identifier, device type, and the status of the binding, linking the login account to the device used for login
/// </summary>
public partial class mst_device_bind
{
    /// <summary>
    /// Unique identifier for each device binding record (Primary Key)
    /// </summary>
    public int device_bind_id { get; set; }

    /// <summary>
    /// The unique identifier of the device (e.g., Android ID, IMEI, or MAC address)
    /// </summary>
    public string device_id { get; set; } = null!;

    public bool? is_deleted { get; set; }

    /// <summary>
    /// User who created the record
    /// </summary>
    public int? creator_id { get; set; }

    /// <summary>
    /// Timestamp when the record was created
    /// </summary>
    public DateOnly? created_at { get; set; }

    /// <summary>
    /// User who updated the record
    /// </summary>
    public int? modifier_id { get; set; }

    /// <summary>
    /// Timestamp when the record was updated
    /// </summary>
    public DateOnly? modified_at { get; set; }
}
