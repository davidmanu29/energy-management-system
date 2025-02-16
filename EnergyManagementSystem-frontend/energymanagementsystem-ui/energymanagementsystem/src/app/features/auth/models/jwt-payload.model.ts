import { JwtPayload } from 'jwt-decode';


export interface CustomJwtPayload extends JwtPayload {
  name?: string;
  actor?: string;
  roles?: string[];
  [key: string]: any;
}
