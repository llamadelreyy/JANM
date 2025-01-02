using System;
using System.Collections.Generic;

namespace PBTPro.DAL.Models;

/// <summary>
/// The &quot;user_accounts&quot; table stores information about user accounts in the system. Each record represents a user account and includes details such as the user&apos;s full name, date of birth, identification information, address, nationality, marital status, and acceptance of terms and conditions.
/// </summary>
public partial class user_account
{
    /// <summary>
    /// This column serves as a unique identifier for each user account and is a foreign key referencing the user associated with the account.
    /// </summary>
    public int ua_user_id { get; set; }

    /// <summary>
    /// Name of the user&apos;s account photo file location/path.
    /// </summary>
    public string? ua_photo_url { get; set; }

    /// <summary>
    /// Name of the user&apos;s account signature file location/path.
    /// </summary>
    public string? ua_signature_url { get; set; }

    /// <summary>
    /// Full name of the user.
    /// </summary>
    public string? ua_name { get; set; }

    /// <summary>
    /// Date of birth of the user. Could be extracted from the identification document (e.g., MyCard).
    /// </summary>
    public DateOnly? dob { get; set; }

    /// <summary>
    /// User gender (e.g., male, female).
    /// </summary>
    public int? gen_id { get; set; }

    /// <summary>
    /// National Identification Number of the user.
    /// </summary>
    public int? nat_id { get; set; }

    /// <summary>
    /// References the race/ethnicity of the user, as identified in the ref_races table.
    /// </summary>
    public int? race_id { get; set; }

    /// <summary>
    /// Flag indicating whether the user is married (true for married, false for single).
    /// </summary>
    public bool? is_married { get; set; }

    /// <summary>
    /// First line of the user&apos;s address.
    /// </summary>
    public string? addr_line1 { get; set; }

    /// <summary>
    /// Second line of the user&apos;s address (optional).
    /// </summary>
    public string? addr_line2 { get; set; }

    /// <summary>
    /// Postal code of the user&apos;s address.
    /// </summary>
    public string? postcode { get; set; }

    /// <summary>
    /// References the town of the user&apos;s address.
    /// </summary>
    public int? town_id { get; set; }

    /// <summary>
    /// References the district of the user&apos;s address.
    /// </summary>
    public int? district_id { get; set; }

    /// <summary>
    /// References the state of the user&apos;s address.
    /// </summary>
    public int? state_id { get; set; }

    /// <summary>
    /// References the country of the user&apos;s address.
    /// </summary>
    public int? country_id { get; set; }

    /// <summary>
    /// Flag indicating whether the user accepted the first set of terms and conditions during registration.
    /// </summary>
    public bool? accept_terms1 { get; set; }

    /// <summary>
    /// Flag indicating whether the user accepted the second set of terms and conditions during registration (if applicable).
    /// </summary>
    public bool? accept_terms2 { get; set; }

    /// <summary>
    /// Flag indicating whether the user accepted the third set of terms and conditions during registration (if applicable).
    /// </summary>
    public bool? accept_terms3 { get; set; }

    /// <summary>
    /// Timestamp indicating when the user account was created.
    /// </summary>
    public DateTime? created_at { get; set; }

    /// <summary>
    /// User ID of the creator who created the account.
    /// </summary>
    public int? creator_id { get; set; }

    /// <summary>
    /// Timestamp indicating when the user account was last modified.
    /// </summary>
    public DateTime? modified_at { get; set; }

    /// <summary>
    /// User ID of the modifier who last updated the account.
    /// </summary>
    public int? modifier_id { get; set; }

    /// <summary>
    /// Flag indicating whether the row has been logically deleted (soft deleted). True indicates deleted, false indicates active.
    /// </summary>
    public bool? is_deleted { get; set; }
}
