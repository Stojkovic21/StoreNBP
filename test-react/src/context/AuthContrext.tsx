import { createContext, useState, PropsWithChildren } from "react";

const AuthProviderContext = createContext<AuthProviderContextValue | undefined>(
  undefined
);

export type AuthProviderContextValue = {
  isAuthenticated: boolean;
  accessToken?: string | null;
  customerId?: string | null;
  role: string;
  handleSignIn: (accessToken: string, customerId: string, role: string) => void;
  handleSignOut: () => void;
};

type AuthProviderProps = PropsWithChildren;

export function AuthProvider({ children }: AuthProviderProps) {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [accessToken, setAccessToken] = useState<string | null | undefined>(
    null
  );
  const [role, setRole] = useState("Guest");
  const [customerId, setCustomerID] = useState<string | null | undefined>();

  function handleSignIn(accessToken: string, customerId: string, role: string) {
    setIsAuthenticated(true);
    setAccessToken(accessToken);
    setRole(role);
    setCustomerID(customerId);
  }

  function handleSignOut() {
    setIsAuthenticated(false);
    setAccessToken(null);
    setRole("Guest");
    setCustomerID(null);
  }

  return (
    <AuthProviderContext
      value={{
        isAuthenticated,
        accessToken,
        handleSignIn,
        handleSignOut,
        role,
        customerId,
      }}
    >
      {children}
    </AuthProviderContext>
  );
}

export default AuthProviderContext;
